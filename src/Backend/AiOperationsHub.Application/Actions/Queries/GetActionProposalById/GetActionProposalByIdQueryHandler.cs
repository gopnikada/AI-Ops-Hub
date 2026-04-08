using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Application.Actions.Dtos;
using MediatR;

namespace AiOperationsHub.Application.Actions.Queries.GetActionProposalById
{
    /// <summary>
    /// Handles retrieval of a single action proposal.
    /// </summary>
    public sealed class GetActionProposalByIdQueryHandler : IRequestHandler<GetActionProposalByIdQuery, ActionProposalDto?>
    {
        private readonly IActionProposalRepository _actionProposalRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetActionProposalByIdQueryHandler"/> class.
        /// </summary>
        /// <param name="actionProposalRepository">The repository used to load action proposals.</param>
        public GetActionProposalByIdQueryHandler(IActionProposalRepository actionProposalRepository)
        {
            _actionProposalRepository = actionProposalRepository;
        }

        /// <summary>
        /// Handles the request to retrieve a single action proposal.
        /// </summary>
        /// <param name="request">The query containing the proposal identifier.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the matching proposal DTO, or <c>null</c> when not found.</returns>
        public async Task<ActionProposalDto?> Handle(
            GetActionProposalByIdQuery request,
            CancellationToken cancellationToken)
        {
            var proposal = await _actionProposalRepository.GetByIdAsync(request.ProposalId, cancellationToken);

            if (proposal is null)
            {
                return null;
            }

            return new ActionProposalDto
            {
                Id = proposal.Id,
                RequestedByUserId = proposal.RequestedByUserId,
                TargetSystem = proposal.TargetSystem,
                ActionName = proposal.ActionName,
                TargetResource = proposal.TargetResource,
                ParametersJson = proposal.ParametersJson,
                PreviewText = proposal.PreviewText,
                RiskLevel = proposal.RiskLevel,
                Status = proposal.Status,
                CreatedAtUtc = proposal.CreatedAtUtc,
                ConfirmedAtUtc = proposal.ConfirmedAtUtc,
                ExecutedAtUtc = proposal.ExecutedAtUtc,
                ExecutionResultJson = proposal.ExecutionResultJson
            };
        }
    }
}