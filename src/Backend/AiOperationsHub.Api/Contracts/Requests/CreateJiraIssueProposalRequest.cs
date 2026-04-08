namespace AiOperationsHub.Api.Contracts.Requests
{
    /// <summary>
    /// Represents the HTTP request body for creating a Jira issue proposal.
    /// </summary>
    public sealed class CreateJiraIssueProposalRequest
    {
        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the Jira project key.
        /// </summary>
        public string ProjectKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira epic key.
        /// </summary>
        public string? EpicKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the issue summary.
        /// </summary>
        public string Summary { get; set; } = null!;

        /// <summary>
        /// Gets or sets the issue description.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional assignee.
        /// </summary>
        public string? Assignee { get; set; }
    }
}