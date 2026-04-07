using System.ComponentModel.DataAnnotations;

namespace AiOperationsHub.Infrastructure.Options
{
    /// <summary>
    /// Represents configuration settings for the OpenAI integration.
    /// </summary>
    public sealed class OpenAiOptions
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public const string SectionName = "OpenAi";

        /// <summary>
        /// Gets or sets the API base URL.
        /// </summary>
        [Required]
        public string BaseUrl { get; set; } = "https://api.openai.com/v1/";

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        [Required]
        public string ApiKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the model name used for requests.
        /// </summary>
        [Required]
        public string Model { get; set; } = null!;

        /// <summary>
        /// Gets or sets the outbound timeout in seconds.
        /// </summary>
        [Range(1, 300)]
        public int TimeoutSeconds { get; set; } = 60;
    }
}