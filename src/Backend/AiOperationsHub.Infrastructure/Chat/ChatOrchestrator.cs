namespace AiOperationsHub.Infrastructure.Chat
{
    using System.Runtime.CompilerServices;
    using AiOperationsHub.Application.Chat;
    using AiOperationsHub.Application.Tools;
    using AiOperationsHub.Application.Tools.Planning;

    /// <summary>
    /// Default backend chat orchestrator for tool-based workflows.
    /// </summary>
    public sealed class ChatOrchestrator : IChatOrchestrator
    {
        private readonly IAiToolPlanner _toolPlanner;
        private readonly IToolRegistry _toolRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatOrchestrator"/> class.
        /// </summary>
        /// <param name="toolPlanner">The AI tool planner.</param>
        /// <param name="toolRegistry">The internal tool registry.</param>
        public ChatOrchestrator(
            IAiToolPlanner toolPlanner,
            IToolRegistry toolRegistry)
        {
            _toolPlanner = toolPlanner;
            _toolRegistry = toolRegistry;
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

            var planningResponse = await _toolPlanner.PlanAsync(
                new ToolPlanningRequest
                {
                    UserMessage = request.Message,
                    Tools = _toolRegistry.GetDefinitions()
                },
                cancellationToken);

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

            var result = await tool.ExecuteAsync(
                planningResponse.Invocation,
                new ToolExecutionContext
                {
                    RequestedByUserId = requestedByUserId,
                    CorrelationId = correlationId,
                    ConversationId = request.ConversationId,
                    OriginalMessage = request.Message
                },
                cancellationToken);

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