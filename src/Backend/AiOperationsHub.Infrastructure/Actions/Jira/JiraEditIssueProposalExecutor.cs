namespace AiOperationsHub.Infrastructure.Actions.Jira
{
    using System.Text.Json;
    using AiOperationsHub.Application.Abstractions.Actions;
    using AiOperationsHub.Application.Abstractions.Jira;
    using AiOperationsHub.Application.Actions.Execution;
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Executes Jira edit-issue action proposals.
    /// </summary>
    public sealed class JiraEditIssueProposalExecutor : IActionProposalExecutor
    {
        private readonly IJiraConnector _jiraConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="JiraEditIssueProposalExecutor"/> class.
        /// </summary>
        /// <param name="jiraConnector">The Jira connector.</param>
        public JiraEditIssueProposalExecutor(IJiraConnector jiraConnector)
        {
            _jiraConnector = jiraConnector;
        }

        /// <inheritdoc />
        public bool CanExecute(ActionProposal proposal)
        {
            return proposal.TargetSystem == ActionTargetSystem.Jira
                && string.Equals(proposal.ActionName, JiraActionType.EditIssue.ToString(), StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public async Task<ActionProposalExecutionResult> ExecuteAsync(
            ActionProposal proposal,
            CancellationToken cancellationToken)
        {
            var parameters = JsonSerializer.Deserialize<UpdateJiraIssueActionParameters>(proposal.ParametersJson);

            if (parameters is null)
            {
                throw new Domain.Common.DomainException("Action proposal parameters are invalid.");
            }

            var result = await _jiraConnector.UpdateIssueAsync(
                new UpdateJiraIssueRequest
                {
                    IssueKey = parameters.IssueKey,
                    Summary = parameters.Summary,
                    Description = parameters.Description,
                    Assignee = parameters.Assignee,
                    Status = parameters.Status
                },
                cancellationToken);

            return new ActionProposalExecutionResult
            {
                ResourceId = result.IssueKey,
                ExecutionResultJson = JsonSerializer.Serialize(new
                {
                    result.IssueKey,
                    result.IssueUrl,
                    result.RawResponseJson
                })
            };
        }
    }
}