namespace AiOperationsHub.Application.Abstractions.Resolution
{
    using AiOperationsHub.Application.Common.Models;

    /// <summary>
    /// Resolves user-provided target references into zero, one, or many concrete resources.
    /// </summary>
    public interface ITargetResourceResolver
    {
        /// <summary>
        /// Resolves a user-provided reference for a target system.
        /// </summary>
        /// <param name="request">The resolution request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The resolution result.</returns>
        Task<ResolveTargetResourceResult> ResolveAsync(
            ResolveTargetResourceRequest request,
            CancellationToken cancellationToken);
    }
}