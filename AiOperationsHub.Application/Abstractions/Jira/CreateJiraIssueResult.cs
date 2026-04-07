namespace AiOperationsHub.Application.Abstractions.Jira
{
    /// <summary>
    /// Represents the result of a Jira issue creation operation.
    /// </summary>
    public sealed class CreateJiraIssueResult
    {
        /// <summary>
        /// Gets or sets the created Jira issue key.
        /// </summary>
        public string IssueKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the URL of the created Jira issue.
        /// </summary>
        public string IssueUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the raw serialized connector response payload.
        /// </summary>
        public string RawResponseJson { get; set; } = null!;
    }
}