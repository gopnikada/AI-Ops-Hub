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

        /// <summary>
        /// Gets or sets the optional Jira issue description text.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the Jira browse URL.
        /// </summary>
        public string? IssueUrl { get; set; }
    }
}