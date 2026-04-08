using MediatR;
using Microsoft.Extensions.Logging;

namespace AiOperationsHub.Application.Common.Behaviors
{
    /// <summary>
    /// Logs the start and completion of MediatR requests for operational observability.
    /// </summary>
    /// <typeparam name="TRequest">The MediatR request type.</typeparam>
    /// <typeparam name="TResponse">The MediatR response type.</typeparam>
    public sealed class RequestLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<RequestLoggingBehavior<TRequest, TResponse>> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestLoggingBehavior{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="logger">The logger used to write request lifecycle messages.</param>
        public RequestLoggingBehavior(ILogger<RequestLoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Logs request processing before and after the next pipeline step executes.
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
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation("Handling application request {RequestName}", requestName);

            var response = await next();

            _logger.LogInformation("Handled application request {RequestName}", requestName);

            return response;
        }
    }
}