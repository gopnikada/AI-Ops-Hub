namespace AiOperationsHub.Infrastructure.Actions.Jira
{
    using AiOperationsHub.Application.Abstractions.Actions;
    using AiOperationsHub.Application.Abstractions.Jira;
    using AiOperationsHub.Application.Actions.Execution;
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;
    using AiOperationsHub.Domain.Common;
    using System.Text.Json;

    /// <summary>
    /// Executes Jira create-issue action proposals.
    /// </summary>
    public sealed class JiraCreateIssueProposalExecutor : IActionProposalExecutor
    {
        private readonly IJiraConnector _jiraConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraCreateIssueProposalExecutor"/> class.
        /// </summary>
        /// <param name="jiraConnector">The Jira connector.</param>
        public JiraCreateIssueProposalExecutor(IJiraConnector jiraConnector)
        {
            _jiraConnector = jiraConnector;
        }

        /// <inheritdoc />
        public bool CanExecute(ActionProposal proposal)
        {
            return proposal.TargetSystem == ActionTargetSystem.Jira
                && string.Equals(
                    proposal.ActionName,
                    JiraActionType.CreateIssue.ToString(),
                    StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public async Task<ActionProposalExecutionResult> ExecuteAsync(
            ActionProposal proposal,
            CancellationToken cancellationToken)
        {
            var parameters = JsonSerializer.Deserialize<CreateJiraIssueActionParameters>(proposal.ParametersJson);

            if (parameters is null)
            {
                throw new DomainException("Action proposal parameters are invalid.");
            }

            var jiraResult = await _jiraConnector.CreateIssueAsync(
                new CreateJiraIssueDraftRequest
                {
                    ProjectKey = parameters.ProjectKey,
                    EpicKey = parameters.EpicKey,
                    Summary = parameters.Summary,
                    Description = parameters.Description,
                    Assignee = parameters.Assignee
                },
                cancellationToken);

            return new ActionProposalExecutionResult
            {
                ResourceId = jiraResult.IssueKey,
                ExecutionResultJson = JsonSerializer.Serialize(new
                {
                    jiraResult.IssueKey,
                    jiraResult.IssueUrl,
                    jiraResult.RawResponseJson
                })
            };
        }
    }
}
}
