using FluentValidation;

namespace AiOperationsHub.Application.Actions.Commands.ConfirmActionProposal
{
    /// <summary>
    /// Validates the input required to confirm and execute an action proposal.
    /// </summary>
    public sealed class ConfirmActionProposalCommandValidator : AbstractValidator<ConfirmActionProposalCommand>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="ConfirmActionProposalCommand"/>.
        /// </summary>
        public ConfirmActionProposalCommandValidator()
        {
            RuleFor(x => x.ProposalId)
                .NotEmpty();

            RuleFor(x => x.ConfirmedByUserId)
                .NotEmpty();

            RuleFor(x => x.CorrelationId)
                .NotEmpty();
        }
    }
}