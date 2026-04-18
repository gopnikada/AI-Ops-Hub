namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents the result of resolving one Jira issue reference.
    /// </summary>
    public sealed class ResolvedJiraIssueResponse
    {
        /// <summary>
        /// Gets or sets the Jira issue key.
        /// </summary>
        public string IssueKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira issue summary.
        /// </summary>
        public string Summary { get; set; } = null!;
    }
}