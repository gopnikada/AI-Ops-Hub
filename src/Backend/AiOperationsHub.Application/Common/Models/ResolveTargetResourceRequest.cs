namespace AiOperationsHub.Application.Common.Models
{
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Represents a generic request to resolve a user-provided target reference.
    /// </summary>
    public sealed class ResolveTargetResourceRequest
    {
        /// <summary>
        /// Gets or sets the target system to resolve against.
        /// </summary>
        public ActionTargetSystem TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the optional scope key used to narrow the search, such as a Jira project key.
        /// </summary>
        public string? ScopeKey { get; set; }

        /// <summary>
        /// Gets or sets the user-provided reference text.
        /// </summary>
        public string Reference { get; set; } = null!;
    }
}