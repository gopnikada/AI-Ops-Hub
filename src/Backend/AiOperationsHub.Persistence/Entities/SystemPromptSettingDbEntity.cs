namespace AiOperationsHub.Persistence.Entities
{
    /// <summary>
    /// Represents one persisted system prompt setting row.
    /// </summary>
    public sealed class SystemPromptSettingDbEntity
    {
        /// <summary>
        /// Gets or sets the unique prompt key.
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