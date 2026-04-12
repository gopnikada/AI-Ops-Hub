namespace AiOperationsHub.Api.Contracts.SystemPrompts
{
    /// <summary>
    /// Represents the incoming request to create or update a system prompt.
    /// </summary>
    public sealed class UpsertSystemPromptRequest
    {
        /// <summary>
        /// Gets or sets the prompt value.
        /// </summary>
        public string Value { get; set; } = null!;
    }
}