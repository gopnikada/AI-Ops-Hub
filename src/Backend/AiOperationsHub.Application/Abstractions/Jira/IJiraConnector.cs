using AiOperationsHub.Application.Common.Models;

namespace AiOperationsHub.Application.Abstractions.Jira
{
    /// <summary>
    /// Defines Jira operations required by the application layer.
    /// </summary>
    public interface IJiraConnector
    {
        /// <summary>
        /// Creates a Jira issue using the supplied normalized request.
        /// </summary>
        /// <param name="request">The request describing the issue to create.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the Jira issue creation result.</returns>
        Task<CreateJiraIssueResult> CreateIssueAsync(
            CreateJiraIssueDraftRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// Resolves a user-provided Jira issue reference to one concrete issue.
        /// </summary>
        /// <param name="request">The issue lookup request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resolved Jira issue.</returns>
        Task<ResolvedJiraIssueResponse> ResolveIssueAsync(
            ResolveJiraIssueRequest request,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the current details of a Jira issue.
        /// </summary>
        /// <param name="issueKey">The Jira issue key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The current Jira issue details.</returns>
        Task<JiraIssueDetailsResponse> GetIssueAsync(
            string issueKey,
            CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing Jira issue.
        /// </summary>
        /// <param name="request">The Jira issue update request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The update result.</returns>
        Task<UpdateJiraIssueResult> UpdateIssueAsync(
            UpdateJiraIssueRequest request,
            CancellationToken cancellationToken);
    }
}