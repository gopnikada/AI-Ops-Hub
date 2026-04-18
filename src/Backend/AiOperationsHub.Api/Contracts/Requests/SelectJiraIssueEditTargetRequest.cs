namespace AiOperationsHub.Api.Contracts.Requests
{
    /// <summary>
    /// Represents the HTTP request body for selecting one Jira issue candidate and creating an edit proposal.
    /// </summary>
    public sealed class SelectJiraIssueEditTargetRequest
    {
        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the optional Jira project key used to narrow issue lookup.
        /// </summary>
        public string? ProjectKey { get; set; }

        /// <summary>
        /// Gets or sets the original user-provided issue reference.
        /// </summary>
        public string IssueReference { get; set; } = null!;

        /// <summary>
        /// Gets or sets the selected Jira issue key.
        /// </summary>
        public string SelectedIssueKey { get; set; } = null!;

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