namespace AiOperationsHub.Application.Abstractions.Providers
{
    /// <summary>
    /// Represents a normalized response returned by an AI provider implementation.
    /// </summary>
    public sealed class AiProviderResponse
    {
        /// <summary>
        /// Gets or sets the primary text output returned by the AI provider.
        /// </summary>
        public string OutputText { get; set; } = null!;

        /// <summary>
        /// Gets or sets the raw serialized response payload returned by the provider, when available.
        /// </summary>
        public string? RawResponseJson { get; set; }
    }
}