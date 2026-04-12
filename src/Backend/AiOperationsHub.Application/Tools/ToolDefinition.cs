namespace AiOperationsHub.Application.Tools
{
    /// <summary>
    /// Describes one tool exposed to the AI orchestration layer.
    /// </summary>
    public sealed class ToolDefinition
    {
        /// <summary>
        /// Gets or sets the unique tool name.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description telling the model when to use the tool.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the JSON schema describing the tool input.
        /// </summary>
        public string InputSchemaJson { get; set; } = null!;

        /// <summary>
        /// Gets or sets the tool safety mode.
        /// </summary>
        public ToolSafetyMode SafetyMode { get; set; }
    }
}