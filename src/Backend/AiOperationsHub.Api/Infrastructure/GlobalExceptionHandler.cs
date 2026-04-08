using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AiOperationsHub.Api.Infrastructure
{
    /// <summary>
    /// Converts unhandled exceptions into RFC 7807 problem details responses.
    /// </summary>
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Attempts to handle an exception and write a problem details response.
        /// </summary>
        /// <param name="httpContext">The current HTTP context.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns><c>true</c> if handled; otherwise <c>false</c>.</returns>
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails;
            var statusCode = StatusCodes.Status500InternalServerError;

            switch (exception)
            {
                case ValidationException validationException:
                    statusCode = StatusCodes.Status400BadRequest;
                    problemDetails = new ValidationProblemDetails(
                        validationException.Errors
                            .GroupBy(error => error.PropertyName)
                            .ToDictionary(
                                group => group.Key,
                                group => group.Select(error => error.ErrorMessage).ToArray()))
                    {
                        Title = "Validation failed.",
                        Status = statusCode,
                        Type = "https://httpstatuses.com/400"
                    };
                    break;

                case KeyNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Resource not found.",
                        Detail = exception.Message,
                        Status = statusCode,
                        Type = "https://httpstatuses.com/404"
                    };
                    break;

                case InvalidOperationException:
                case ArgumentException:
                    statusCode = StatusCodes.Status409Conflict;
                    problemDetails = new ProblemDetails
                    {
                        Title = "Request could not be completed.",
                        Detail = exception.Message,
                        Status = statusCode,
                        Type = "https://httpstatuses.com/409"
                    };
                    break;

                default:
                    problemDetails = new ProblemDetails
                    {
                        Title = "An unexpected error occurred.",
                        Detail = "The server encountered an unexpected condition.",
                        Status = statusCode,
                        Type = "https://httpstatuses.com/500"
                    };
                    break;
            }

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            _logger.LogError(
                exception,
                "Unhandled exception for request {Method} {Path}. TraceId: {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.TraceIdentifier);

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}