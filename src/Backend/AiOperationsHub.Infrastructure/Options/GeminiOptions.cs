using System.ComponentModel.DataAnnotations;

namespace AiOperationsHub.Infrastructure.Options
{
    /// <summary>
    /// Represents configuration settings for the Gemini API integration.
    /// </summary>
    public sealed class GeminiOptions
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public const string SectionName = "Gemini";

        /// <summary>
        /// Gets or sets the Gemini API base URL.
        /// </summary>
        [Required]
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com/";

        /// <summary>
        /// Gets or sets the Gemini API key.
        /// </summary>
        [Required]
        public string ApiKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Gemini model name.
        /// </summary>
        [Required]
        public string Model { get; set; } = "gemini-3-flash-preview";

        /// <summary>
        /// Gets or sets the outbound timeout in seconds.
        /// </summary>
        [Range(1, 300)]
        public int TimeoutSeconds { get; set; } = 60;

        /// <summary>
        /// Gets or sets the application-defined token budget used for usage percentages.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int BudgetTokens { get; set; } = 1000000;
    }
}