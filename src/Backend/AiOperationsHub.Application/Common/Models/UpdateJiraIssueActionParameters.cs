namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents the parameters required to edit an existing Jira issue.
    /// </summary>
    public sealed class UpdateJiraIssueActionParameters
    {
        /// <summary>
        /// Gets or sets the optional project key used to narrow issue lookup.
        /// </summary>
        public string? ProjectKey { get; set; }

        /// <summary>
        /// Gets or sets the original user-provided issue reference, which may be an issue key or free-text description.
        /// </summary>
        public string IssueReference { get; set; } = null!;

        /// <summary>
        /// Gets or sets the resolved Jira issue key.
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

        /// <summary>
        /// Gets or sets the structured list of planned changes.
        /// </summary>
        public IReadOnlyCollection<JiraIssueFieldChange> ChangeSet { get; set; } = Array.Empty<JiraIssueFieldChange>();
    }
}