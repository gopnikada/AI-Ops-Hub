using FluentValidation;

namespace AiOperationsHub.Application.Actions.Commands.CreateJiraIssueProposal
{
    /// <summary>
    /// Validates the input required to create a Jira issue proposal.
    /// </summary>
    public sealed class CreateJiraIssueProposalCommandValidator : AbstractValidator<CreateJiraIssueProposalCommand>
    {
        /// <summary>
        /// Initializes validation rules for <see cref="CreateJiraIssueProposalCommand"/>.
        /// </summary>
        public CreateJiraIssueProposalCommandValidator()
        {
            RuleFor(x => x.RequestedByUserId)
                .NotEmpty();

            RuleFor(x => x.CorrelationId)
                .NotEmpty();

            RuleFor(x => x.ProjectKey)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.EpicKey)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Summary)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Description)
                .NotNull();

            RuleFor(x => x.Assignee)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Assignee));
        }
    }
}