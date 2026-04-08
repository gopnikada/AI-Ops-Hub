using FluentValidation;
using MediatR;

namespace AiOperationsHub.Application.Common.Behaviors
{
    /// <summary>
    /// Executes FluentValidation validators for the current request before it reaches the handler.
    /// </summary>
    /// <typeparam name="TRequest">The MediatR request type.</typeparam>
    /// <typeparam name="TResponse">The MediatR response type.</typeparam>
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="validators">The validators registered for the current request type.</param>
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        /// <summary>
        /// Validates the request and throws a <see cref="ValidationException"/> when validation fails.
        /// </summary>
        /// <param name="request">The incoming MediatR request.</param>
        /// <param name="next">The next delegate in the MediatR pipeline.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the handler response.</returns>
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(x => x.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .SelectMany(x => x.Errors)
                    .Where(x => x is not null)
                    .ToList();

                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }

            return await next();
        }
    }
}