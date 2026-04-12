namespace AiOperationsHub.Application.Tools.Planning
{
    using AiOperationsHub.Application.Tools;

    /// <summary>
    /// Represents the normalized output of AI tool selection.
    /// </summary>
    public sealed class ToolPlanningResponse
    {
        /// <summary>
        /// Gets or sets the direct assistant message when no tool is required.
        /// </summary>
        public string? AssistantMessage { get; set; }

        /// <summary>
        /// Gets or sets the selected tool invocation.
        /// </summary>
        public ToolInvocation? Invocation { get; set; }

        /// <summary>
        /// Gets or sets the raw provider response JSON.
        /// </summary>
        public string? RawResponseJson { get; set; }
    }
}