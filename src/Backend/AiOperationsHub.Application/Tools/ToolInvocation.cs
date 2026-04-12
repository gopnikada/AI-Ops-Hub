namespace AiOperationsHub.Application.Tools
{
    /// <summary>
    /// Represents one AI-selected tool invocation.
    /// </summary>
    public sealed class ToolInvocation
    {
        /// <summary>
        /// Gets or sets the selected tool name.
        /// </summary>
        public string ToolName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the raw JSON arguments.
        /// </summary>
        public string ArgumentsJson { get; set; } = "{}";
    }
}