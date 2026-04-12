namespace AiOperationsHub.Infrastructure.Tools.Jira
{
    using System.Text.Json;
    using AiOperationsHub.Application.Actions.Commands.CreateJiraIssueProposal;
    using AiOperationsHub.Application.Tools;
    using MediatR;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Creates a Jira issue proposal from an AI-selected tool invocation.
    /// </summary>
    public sealed class JiraProposeCreateIssueTool : IChatTool
    {
        private const string InputSchema =
            """
            {
              "type": "object",
              "properties": {
                "projectKey": {
                  "type": "string",
                  "description": "Jira project key, for example SCRUM."
                },
                "summary": {
                  "type": "string",
                  "description": "Short issue summary."
                },
                "description": {
                  "type": "string",
                  "description": "Detailed issue description."
                },
                "epicKey": {
                  "type": "string",
                  "description": "Optional Jira epic key."
                },
                "assignee": {
                  "type": "string",
                  "description": "Optional Jira assignee."
                }
              },
              "required": ["projectKey", "summary", "description"]
            }
            """;

        private readonly ISender _sender;
        private readonly ILogger<JiraProposeCreateIssueTool> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraProposeCreateIssueTool"/> class.
        /// </summary>
        /// <param name="sender">The MediatR sender.</param>
        /// <param name="logger">The logger.</param>
        public JiraProposeCreateIssueTool(
            ISender sender,
            ILogger<JiraProposeCreateIssueTool> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        /// <inheritdoc />
        public ToolDefinition Definition => new()
        {
            Name = "jira_propose_create_issue",
            Description =
                "Create a proposal for a new Jira issue. " +
                "Use when the user asks to create, open, file, or raise a Jira ticket. " +
                "This tool creates a proposal only and does not create the real Jira issue directly.",
            InputSchemaJson = InputSchema,
            SafetyMode = ToolSafetyMode.RequiresConfirmation
        };

        /// <inheritdoc />
        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolInvocation invocation,
            ToolExecutionContext context,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Starting Jira proposal tool execution. CorrelationId: {CorrelationId}, ArgumentsJson: {ArgumentsJson}",
                    context.CorrelationId,
                    invocation.ArgumentsJson);

                var payload = JsonSerializer.Deserialize<Payload>(
                    invocation.ArgumentsJson,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

                if (payload is null)
                {
                    throw new InvalidOperationException("The Jira proposal payload could not be parsed.");
                }

                _logger.LogInformation(
                    "Parsed Jira proposal payload. ProjectKey: {ProjectKey}, Summary: {Summary}, EpicKey: {EpicKey}, Assignee: {Assignee}",
                    payload.ProjectKey,
                    payload.Summary,
                    payload.EpicKey,
                    payload.Assignee);

                var command = new CreateJiraIssueProposalCommand
                {
                    RequestedByUserId = context.RequestedByUserId,
                    CorrelationId = context.CorrelationId,
                    ConversationId = context.ConversationId,
                    ProjectKey = payload.ProjectKey,
                    EpicKey = payload.EpicKey,
                    Summary = payload.Summary,
                    Description = payload.Description,
                    Assignee = payload.Assignee
                };

                var proposal = await _sender.Send(command, cancellationToken);

                _logger.LogInformation(
                    "Jira proposal created successfully. ProposalId: {ProposalId}, CorrelationId: {CorrelationId}",
                    proposal.Id,
                    context.CorrelationId);

                return new ToolExecutionResult
                {
                    ToolName = Definition.Name,
                    Message = "Created Jira issue proposal.",
                    Proposal = proposal
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Jira proposal tool execution failed. CorrelationId: {CorrelationId}, ArgumentsJson: {ArgumentsJson}",
                    context.CorrelationId,
                    invocation.ArgumentsJson);

                throw;
            }
        }

        private sealed class Payload
        {
            public string ProjectKey { get; set; } = null!;

            public string Summary { get; set; } = null!;

            public string Description { get; set; } = null!;

            public string? EpicKey { get; set; }

            public string? Assignee { get; set; }
        }
    }
}