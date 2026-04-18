namespace AiOperationsHub.Application.TargetResolution.Queries.ResolveTargetResource
{
    using AiOperationsHub.Application.Abstractions.Resolution;
    using AiOperationsHub.Application.Common.Models;
    using MediatR;

    /// <summary>
    /// Handles target-resource resolution queries.
    /// </summary>
    public sealed class ResolveTargetResourceQueryHandler
        : IRequestHandler<ResolveTargetResourceQuery, ResolveTargetResourceResult>
    {
        private readonly ITargetResourceResolver _targetResourceResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolveTargetResourceQueryHandler"/> class.
        /// </summary>
        /// <param name="targetResourceResolver">The generic target resource resolver.</param>
        public ResolveTargetResourceQueryHandler(ITargetResourceResolver targetResourceResolver)
        {
            _targetResourceResolver = targetResourceResolver;
        }

        /// <inheritdoc />
        public Task<ResolveTargetResourceResult> Handle(
            ResolveTargetResourceQuery request,
            CancellationToken cancellationToken)
        {
            return _targetResourceResolver.ResolveAsync(
                new ResolveTargetResourceRequest
                {
                    TargetSystem = request.TargetSystem,
                    ScopeKey = request.ScopeKey,
                    Reference = request.Reference
                },
                cancellationToken);
        }
    }
}