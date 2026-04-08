namespace AiOperationsHub.Infrastructure.Options
{
    /// <summary>
    /// Represents configuration settings for structured anonymization.
    /// </summary>
    public sealed class AnonymizationOptions
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public const string SectionName = "Anonymization";

        /// <summary>
        /// Gets or sets a value indicating whether structured anonymization is enabled.
        /// </summary>
        public bool EnableStructuredAnonymization { get; set; } = true;
    }
}