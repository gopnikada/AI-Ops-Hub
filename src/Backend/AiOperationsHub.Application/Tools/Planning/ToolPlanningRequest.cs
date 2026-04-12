namespace AiOperationsHub.Application.Tools.Planning
{
    using AiOperationsHub.Application.Tools;

    /// <summary>
    /// Represents a tool-planning request sent to the AI layer.
    /// </summary>
    public sealed class ToolPlanningRequest
    {
        /// <summary>
        /// Gets or sets the user message.
        /// </summary>
        public string UserMessage { get; set; } = null!;

        /// <summary>
        /// Gets or sets the available tools.
        /// </summary>
        public IReadOnlyCollection<ToolDefinition> Tools { get; set; } = Array.Empty<ToolDefinition>();
    }
}