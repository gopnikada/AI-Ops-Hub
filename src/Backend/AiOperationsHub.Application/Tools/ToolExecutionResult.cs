namespace AiOperationsHub.Application.Tools
{
    using AiOperationsHub.Application.Actions.Dtos;

    /// <summary>
    /// Represents the result of executing a tool.
    /// </summary>
    public sealed class ToolExecutionResult
    {
        /// <summary>
        /// Gets or sets the tool name.
        /// </summary>
        public string ToolName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional assistant message.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the optional proposal returned by the tool.
        /// </summary>
        public ActionProposalDto? Proposal { get; set; }

        /// <summary>
        /// Gets or sets optional structured data as JSON.
        /// </summary>
        public string? DataJson { get; set; }
    }
}