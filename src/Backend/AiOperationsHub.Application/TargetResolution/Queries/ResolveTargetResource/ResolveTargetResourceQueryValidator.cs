namespace AiOperationsHub.Application.TargetResolution.Queries.ResolveTargetResource
{
    using FluentValidation;

    /// <summary>
    /// Validates <see cref="ResolveTargetResourceQuery"/>.
    /// </summary>
    public sealed class ResolveTargetResourceQueryValidator : AbstractValidator<ResolveTargetResourceQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveTargetResourceQueryValidator"/> class.
        /// </summary>
        public ResolveTargetResourceQueryValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty()
                .MaximumLength(256);
        }
    }
}