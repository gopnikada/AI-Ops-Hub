namespace AiOperationsHub.Api.Contracts.Requests
{
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Represents the HTTP request body for resolving a target resource reference.
    /// </summary>
    public sealed class ResolveTargetResourceRequest
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