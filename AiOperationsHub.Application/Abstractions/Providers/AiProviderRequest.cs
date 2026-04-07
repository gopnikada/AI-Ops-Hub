using AiOperationsHub.Domain.Ai;

namespace AiOperationsHub.Application.Abstractions.Providers
{
    /// <summary>
    /// Represents a normalized request sent to an AI provider implementation.
    /// </summary>
    public sealed class AiProviderRequest
    {
        /// <summary>
        /// Gets or sets the target AI provider type.
        /// </summary>
        public AiProviderType ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the system prompt used to guide provider behavior.
        /// </summary>
        public string SystemPrompt { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user prompt or sanitized input content sent to the provider.
        /// </summary>
        public string UserPrompt { get; set; } = null!;

        /// <summary>
        /// Gets or sets an optional correlation identifier used for tracing provider requests.
        /// </summary>
        public string? CorrelationId { get; set; }
    }
}