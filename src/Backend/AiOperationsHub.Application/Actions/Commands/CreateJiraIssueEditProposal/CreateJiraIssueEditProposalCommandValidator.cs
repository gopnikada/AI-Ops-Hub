namespace AiOperationsHub.Application.Actions.Commands.CreateJiraIssueEditProposal
{
    using FluentValidation;

    /// <summary>
    /// Validates <see cref="CreateJiraIssueEditProposalCommand"/>.
    /// </summary>
    public sealed class CreateJiraIssueEditProposalCommandValidator : AbstractValidator<CreateJiraIssueEditProposalCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJiraIssueEditProposalCommandValidator"/> class.
        /// </summary>
        public CreateJiraIssueEditProposalCommandValidator()
        {
            RuleFor(x => x.IssueReference)
                .NotEmpty()
                .MaximumLength(256);

            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.Summary) ||
                    !string.IsNullOrWhiteSpace(x.Description) ||
                    !string.IsNullOrWhiteSpace(x.Assignee) ||
                    !string.IsNullOrWhiteSpace(x.Status))
                .WithMessage("At least one Jira field change must be provided.");
        }
    }
}