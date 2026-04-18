namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents an issue lookup request used to resolve a Jira issue key from a user-provided reference.
    /// </summary>
    public sealed class ResolveJiraIssueRequest
    {
        /// <summary>
        /// Gets or sets the optional project key used to narrow the lookup.
        /// </summary>
        public string? ProjectKey { get; set; }

        /// <summary>
        /// Gets or sets the user-provided issue reference, which may be an issue key or free-text description.
        /// </summary>
        public string IssueReference { get; set; } = null!;
    }
}