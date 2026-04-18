namespace AiOperationsHub.Application.Common.Models
{
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Represents the result of resolving a user-provided target reference.
    /// </summary>
    public sealed class ResolveTargetResourceResult
    {
        /// <summary>
        /// Gets or sets the target system that was resolved.
        /// </summary>
        public ActionTargetSystem TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the optional scope key used during resolution.
        /// </summary>
        public string? ScopeKey { get; set; }

        /// <summary>
        /// Gets or sets the original user-provided reference text.
        /// </summary>
        public string Reference { get; set; } = null!;

        /// <summary>
        /// Gets or sets the resolution status.
        /// </summary>
        public TargetResourceResolutionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the resolved identifier when exactly one match is available.
        /// </summary>
        public string? ResolvedIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the candidate matches returned by resolution.
        /// </summary>
        public IReadOnlyCollection<ResolvedTargetOption> Matches { get; set; } = Array.Empty<ResolvedTargetOption>();
    }
}