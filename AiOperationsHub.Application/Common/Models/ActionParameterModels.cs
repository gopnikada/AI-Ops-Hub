namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents the normalized action parameters for a Jira issue creation proposal.
    /// </summary>
    public sealed class CreateJiraIssueActionParameters
    {
        /// <summary>
        /// Gets or sets the Jira project key.
        /// </summary>
        public string ProjectKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira epic key.
        /// </summary>
        public string EpicKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira issue summary.
        /// </summary>
        public string Summary { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira issue description.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional Jira assignee.
        /// </summary>
        public string? Assignee { get; set; }
    }
}