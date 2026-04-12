namespace AiOperationsHub.Application.Prompts.Commands.UpsertSystemPrompt
{
    using FluentValidation;

    /// <summary>
    /// Validates <see cref="UpsertSystemPromptCommand"/>.
    /// </summary>
    public sealed class UpsertSystemPromptCommandValidator : AbstractValidator<UpsertSystemPromptCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpsertSystemPromptCommandValidator"/> class.
        /// </summary>
        public UpsertSystemPromptCommandValidator()
        {
            RuleFor(x => x.Key)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(x => x.Value)
                .NotEmpty()
                .MaximumLength(20000);
        }
    }
}