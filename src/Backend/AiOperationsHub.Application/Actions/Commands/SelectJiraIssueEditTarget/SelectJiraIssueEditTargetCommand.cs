namespace AiOperationsHub.Application.Actions.Commands.SelectJiraIssueEditTarget
{
    using AiOperationsHub.Application.Actions.Dtos;
    using MediatR;

    /// <summary>
    /// Creates a Jira edit proposal after the caller has selected one concrete Jira issue candidate.
    /// </summary>
    public sealed class SelectJiraIssueEditTargetCommand : IRequest<ActionProposalDto>
    {
        /// <summary>
        /// Gets or sets the requesting user identifier.
        /// </summary>
        public Guid RequestedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the optional project key used to narrow lookup.
        /// </summary>
        public string? ProjectKey { get; set; }

        /// <summary>
        /// Gets or sets the original user-provided reference.
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