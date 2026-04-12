namespace AiOperationsHub.Application.Prompts.Dtos
{
    /// <summary>
    /// Represents a system prompt setting returned to API consumers.
    /// </summary>
    public sealed class SystemPromptDto
    {
        /// <summary>
        /// Gets or sets the prompt key.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Gets or sets the prompt value.
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user identifier that last updated the prompt.
        /// </summary>
        public Guid? UpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp of the last update.
        /// </summary>
        public DateTime UpdatedUtc { get; set; }
    }
}