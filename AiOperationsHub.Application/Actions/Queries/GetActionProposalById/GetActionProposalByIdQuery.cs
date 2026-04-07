using AiOperationsHub.Application.Actions.Dtos;
using MediatR;

namespace AiOperationsHub.Application.Actions.Queries.GetActionProposalById
{
    /// <summary>
    /// Retrieves a single action proposal by its unique identifier.
    /// </summary>
    public sealed class GetActionProposalByIdQuery : IRequest<ActionProposalDto?>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the requested proposal.
        /// </summary>
        public Guid ProposalId { get; set; }
    }
}