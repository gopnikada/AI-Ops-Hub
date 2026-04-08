using AiOperationsHub.Application.Actions.Dtos;
using MediatR;

namespace AiOperationsHub.Application.Actions.Commands.ConfirmActionProposal
{
    /// <summary>
    /// Confirms and executes an existing action proposal.
    /// </summary>
    public sealed class ConfirmActionProposalCommand : IRequest<ActionProposalDto>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the proposal to confirm.
        /// </summary>
        public Guid ProposalId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user confirming the proposal.
        /// </summary>
        public Guid ConfirmedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier for the current request flow.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the optional conversation identifier associated with the request.
        /// </summary>
        public Guid? ConversationId { get; set; }
    }
}