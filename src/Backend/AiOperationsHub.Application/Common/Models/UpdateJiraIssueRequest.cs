namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents a Jira issue update request.
    /// </summary>
    public sealed class UpdateJiraIssueRequest
    {
        /// <summary>
        /// Gets or sets the Jira issue key to edit.
        /// </summary>
        public string IssueKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional new summary.
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Gets or sets the optional new description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the optional new assignee identifier.
        /// </summary>
        public string? Assignee { get; set; }

        /// <summary>
        /// Gets or sets the optional target status name.
        /// </summary>
        public string? Status { get; set; }
    }
}