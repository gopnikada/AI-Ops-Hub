namespace AiOperationsHub.Infrastructure.Tools.Jira
{
    using System.Text.Json;
    using AiOperationsHub.Application.Actions.Commands.CreateJiraIssueEditProposal;
    using AiOperationsHub.Application.Tools;
    using MediatR;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Creates a Jira issue edit proposal from an AI-selected tool invocation.
    /// </summary>
    public sealed class JiraProposeEditIssueTool : IChatTool
    {
        private const string InputSchema =
            """
            {
              "type": "object",
              "properties": {
                "projectKey": {
                  "type": "string",
                  "description": "Optional Jira project key used to narrow issue lookup."
                },
                "issueReference": {
                  "type": "string",
                  "description": "The Jira issue key like SCRUM-123, or a free-text reference describing the issue to update."
                },
                "summary": {
                  "type": "string",
                  "description": "Optional new issue summary."
                },
                "description": {
                  "type": "string",
                  "description": "Optional new issue description."
                },
                "assignee": {
                  "type": "string",
                  "description": "Optional new assignee."
                },
                "status": {
                  "type": "string",
                  "description": "Optional target status name."
                }
              },
              "required": ["issueReference"]
            }
            """;

        private readonly ISender _sender;
        private readonly ILogger<JiraProposeEditIssueTool> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraProposeEditIssueTool"/> class.
        /// </summary>
        /// <param name="sender">The MediatR sender.</param>
        /// <param name="logger">The logger.</param>
        public JiraProposeEditIssueTool(
            ISender sender,
            ILogger<JiraProposeEditIssueTool> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        /// <inheritdoc />
        public ToolDefinition Definition => new()
        {
            Name = "jira_propose_edit_issue",
            Description =
                "Create a proposal for editing an existing Jira issue. " +
                "Use when the user asks to change an issue summary, description, assignee, or status. " +
                "This tool creates a proposal only and does not update the real Jira issue directly.",
            InputSchemaJson = InputSchema,
            SafetyMode = ToolSafetyMode.RequiresConfirmation
        };

        /// <inheritdoc />
        public async Task<ToolExecutionResult> ExecuteAsync(
            ToolInvocation invocation,
            ToolExecutionContext context,
            CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Deserialize<Payload>(
                invocation.ArgumentsJson,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (payload is null)
            {
                throw new InvalidOperationException("The Jira edit proposal payload could not be parsed.");
            }

            _logger.LogInformation(
                "Creating Jira edit proposal. CorrelationId: {CorrelationId}, IssueReference: {IssueReference}",
                context.CorrelationId,
                payload.IssueReference);

            var proposal = await _sender.Send(
                new CreateJiraIssueEditProposalCommand
                {
                    RequestedByUserId = context.RequestedByUserId,
                    CorrelationId = context.CorrelationId,
                    ConversationId = context.ConversationId,
                    ProjectKey = payload.ProjectKey,
                    IssueReference = payload.IssueReference,
                    Summary = payload.Summary,
                    Description = payload.Description,
                    Assignee = payload.Assignee,
                    Status = payload.Status
                },
                cancellationToken);

            return new ToolExecutionResult
            {
                ToolName = Definition.Name,
                Message = "Created Jira issue edit proposal.",
                Proposal = proposal
            };
        }

        private sealed class Payload
        {
            public string? ProjectKey { get; set; }

            public string IssueReference { get; set; } = null!;

            public string? Summary { get; set; }

            public string? Description { get; set; }

            public string? Assignee { get; set; }

            public string? Status { get; set; }
        }
    }
}