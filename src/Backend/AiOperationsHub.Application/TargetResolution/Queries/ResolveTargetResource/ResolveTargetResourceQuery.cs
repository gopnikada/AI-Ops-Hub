namespace AiOperationsHub.Application.TargetResolution.Queries.ResolveTargetResource
{
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;
    using MediatR;

    /// <summary>
    /// Resolves a user-provided target reference into one or more concrete resources.
    /// </summary>
    public sealed class ResolveTargetResourceQuery : IRequest<ResolveTargetResourceResult>
    {
        /// <summary>
        /// Gets or sets the target system to resolve against.
        /// </summary>
        public ActionTargetSystem TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the optional scope key used to narrow the search.
        /// </summary>
        public string? ScopeKey { get; set; }

        /// <summary>
        /// Gets or sets the user-provided reference text.
        /// </summary>
        public string Reference { get; set; } = null!;
    }
}