namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents the current editable details of a Jira issue.
    /// </summary>
    public sealed class JiraIssueDetailsResponse
    {
        /// <summary>
        /// Gets or sets the Jira issue key.
        /// </summary>
        public string IssueKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current summary.
        /// </summary>
        public string Summary { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current description as plain text when available.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the current assignee display value or account identifier.
        /// </summary>
        public string? Assignee { get; set; }

        /// <summary>
        /// Gets or sets the current status name.
        /// </summary>
        public string Status { get; set; } = null!;
    }
}