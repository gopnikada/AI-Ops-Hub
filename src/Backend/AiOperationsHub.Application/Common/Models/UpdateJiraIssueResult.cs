namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents the result of updating an existing Jira issue.
    /// </summary>
    public sealed class UpdateJiraIssueResult
    {
        /// <summary>
        /// Gets or sets the Jira issue key.
        /// </summary>
        public string IssueKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira issue URL.
        /// </summary>
        public string IssueUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the raw response payload used for diagnostics.
        /// </summary>
        public string RawResponseJson { get; set; } = null!;
    }
}