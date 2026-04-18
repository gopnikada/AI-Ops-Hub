namespace AiOperationsHub.Api.Contracts.Requests
{
    /// <summary>
    /// Represents the HTTP request body for creating a Jira issue edit proposal.
    /// </summary>
    public sealed class CreateJiraIssueEditProposalRequest
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
        /// Gets or sets the user-provided issue reference, which may be an issue key or free-text reference.
        /// </summary>
        public string IssueReference { get; set; } = null!;

        /// <summary>
        /// Gets or sets the already selected Jira issue key, when the user has chosen one candidate from a prior resolution step.
        /// </summary>
        public string? ResolvedIssueKey { get; set; }

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