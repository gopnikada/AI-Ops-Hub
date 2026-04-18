namespace AiOperationsHub.Application.Actions.Commands.SelectJiraIssueEditTarget
{
    using FluentValidation;

    /// <summary>
    /// Validates <see cref="SelectJiraIssueEditTargetCommand"/>.
    /// </summary>
    public sealed class SelectJiraIssueEditTargetCommandValidator : AbstractValidator<SelectJiraIssueEditTargetCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectJiraIssueEditTargetCommandValidator"/> class.
        /// </summary>
        public SelectJiraIssueEditTargetCommandValidator()
        {
            RuleFor(x => x.IssueReference)
                .NotEmpty()
                .MaximumLength(256);

            RuleFor(x => x.SelectedIssueKey)
                .NotEmpty()
                .MaximumLength(64);

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