using AiOperationsHub.Application.Actions.Dtos;
using MediatR;

namespace AiOperationsHub.Application.Actions.Commands.CreateJiraIssueProposal
{
    /// <summary>
    /// Creates a Jira issue action proposal that can later be previewed and confirmed.
    /// </summary>
    public sealed class CreateJiraIssueProposalCommand : IRequest<ActionProposalDto>
    {
        /// <summary>
        /// Gets or sets the identifier of the user requesting the proposal.
        /// </summary>
        public Guid RequestedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier for the current request flow.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the optional conversation identifier associated with the request.
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
        /// Gets or sets the summary for the proposed Jira issue.
        /// </summary>
        public string Summary { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description for the proposed Jira issue.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional assignee for the proposed Jira issue.
        /// </summary>
        public string? Assignee { get; set; }
    }
}