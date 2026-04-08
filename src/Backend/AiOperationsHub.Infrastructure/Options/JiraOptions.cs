using System.ComponentModel.DataAnnotations;

namespace AiOperationsHub.Infrastructure.Options
{
    /// <summary>
    /// Represents configuration settings for the Jira integration.
    /// </summary>
    public sealed class JiraOptions
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public const string SectionName = "Jira";

        /// <summary>
        /// Gets or sets the Jira base URL.
        /// </summary>
        [Required]
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira service account email.
        /// </summary>
        [Required]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Jira API token.
        /// </summary>
        [Required]
        public string ApiToken { get; set; } = null!;

        /// <summary>
        /// Gets or sets the default Jira project key.
        /// </summary>
        [Required]
        public string DefaultProjectKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the outbound timeout in seconds.
        /// </summary>
        [Range(1, 300)]
        public int TimeoutSeconds { get; set; } = 30;
    }
}