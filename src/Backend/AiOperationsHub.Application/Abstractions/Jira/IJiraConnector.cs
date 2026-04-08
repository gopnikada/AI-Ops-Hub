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
    }
}