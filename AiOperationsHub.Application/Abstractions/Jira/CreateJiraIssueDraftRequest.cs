namespace AiOperationsHub.Application.Abstractions.Jira
{
    /// <summary>
    /// Represents the normalized input required to create a Jira issue.
    /// </summary>
    public sealed class CreateJiraIssueDraftRequest
    {
        /// <summary>
        /// Gets or sets the Jira project key.
        /// </summary>
        public string ProjectKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira epic key under which the issue will be created.
        /// </summary>
        public string EpicKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the issue summary.
        /// </summary>
        public string Summary { get; set; } = null!;

        /// <summary>
        /// Gets or sets the issue description.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional assignee identifier or name.
        /// </summary>
        public string? Assignee { get; set; }
    }
}