namespace AiOperationsHub.Infrastructure.Chat
{
    using System.Runtime.CompilerServices;
    using AiOperationsHub.Application.Chat;
    using AiOperationsHub.Application.Tools;
    using AiOperationsHub.Application.Tools.Planning;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Default backend chat orchestrator for tool-based workflows.
    /// </summary>
    public sealed class ChatOrchestrator : IChatOrchestrator
    {
        private readonly IAiToolPlanner _toolPlanner;
        private readonly IToolRegistry _toolRegistry;
        private readonly ILogger<ChatOrchestrator> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatOrchestrator"/> class.
        /// </summary>
        /// <param name="toolPlanner">The AI tool planner.</param>
        /// <param name="toolRegistry">The internal tool registry.</param>
        /// <param name="logger">The logger.</param>
        public ChatOrchestrator(
            IAiToolPlanner toolPlanner,
            IToolRegistry toolRegistry,
            ILogger<ChatOrchestrator> logger)
        {
            _toolPlanner = toolPlanner;
            _toolRegistry = toolRegistry;
            _logger = logger;
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ChatStreamEvent> StreamAsync(
            ChatStreamRequest request,
            Guid requestedByUserId,
            Guid correlationId,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            yield return ChatStreamEvent.Create(
                ChatStreamEventType.MessageReceived,
                "Request received.");

            yield return ChatStreamEvent.Create(
                ChatStreamEventType.AnalysisStarted,
                "Analyzing request.");

            ToolPlanningResponse? planningResponse = null;
            string? planningError = null;

            try
            {
                planningResponse = await _toolPlanner.PlanAsync(
                    new ToolPlanningRequest
                    {
                        UserMessage = request.Message,
                        Tools = _toolRegistry.GetDefinitions()
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Tool planning failed. CorrelationId: {CorrelationId}, UserId: {UserId}, Message: {Message}",
                    correlationId,
                    requestedByUserId,
                    request.Message);

                planningError = $"Tool planning failed: {ex.Message}";
            }

            if (planningError is not null)
            {
                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.Error,
                    Message = planningError
                };

                yield break;
            }

            if (planningResponse is null)
            {
                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.Error,
                    Message = "Tool planning failed: no planning response was returned."
                };

                yield break;
            }

            if (planningResponse.Invocation is null)
            {
                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.AssistantMessage,
                    Message = planningResponse.AssistantMessage ?? "No action was selected."
                };

                yield break;
            }

            var tool = _toolRegistry.Find(planningResponse.Invocation.ToolName);
            if (tool is null)
            {
                _logger.LogError(
                    "Tool lookup failed. CorrelationId: {CorrelationId}, ToolName: {ToolName}",
                    correlationId,
                    planningResponse.Invocation.ToolName);

                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.Error,
                    Message = $"Unknown tool: {planningResponse.Invocation.ToolName}"
                };

                yield break;
            }

            yield return new ChatStreamEvent
            {
                Type = ChatStreamEventType.ToolSelected,
                ToolName = tool.Definition.Name,
                Message = $"Selected tool {tool.Definition.Name}."
            };

            ToolExecutionResult? result = null;
            string? executionError = null;

            try
            {
                result = await tool.ExecuteAsync(
                    planningResponse.Invocation,
                    new ToolExecutionContext
                    {
                        RequestedByUserId = requestedByUserId,
                        CorrelationId = correlationId,
                        ConversationId = request.ConversationId,
                        OriginalMessage = request.Message
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Tool execution failed. CorrelationId: {CorrelationId}, ToolName: {ToolName}, ArgumentsJson: {ArgumentsJson}",
                    correlationId,
                    planningResponse.Invocation.ToolName,
                    planningResponse.Invocation.ArgumentsJson);

                executionError = $"Tool execution failed: {ex.Message}";
            }

            if (executionError is not null)
            {
                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.Error,
                    Message = executionError
                };

                yield break;
            }

            if (result is null)
            {
                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.Error,
                    Message = "Tool execution failed: no result was returned."
                };

                yield break;
            }

            if (result.Proposal is not null)
            {
                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.ProposalReady,
                    ProposalId = result.Proposal.Id,
                    Proposal = result.Proposal,
                    Message = result.Message
                };

                yield return new ChatStreamEvent
                {
                    Type = ChatStreamEventType.ConfirmationRequired,
                    ProposalId = result.Proposal.Id,
                    Message = "Confirmation is required before execution."
                };

                yield break;
            }

            yield return new ChatStreamEvent
            {
                Type = ChatStreamEventType.AssistantMessage,
                Message = result.Message,
                DataJson = result.DataJson
            };
        }
    }
}