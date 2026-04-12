namespace AiOperationsHub.Application.Tools
{
    /// <summary>
    /// Defines one internally executable chat tool.
    /// </summary>
    public interface IChatTool
    {
        /// <summary>
        /// Gets the tool definition exposed to the orchestration layer.
        /// </summary>
        ToolDefinition Definition { get; }

        /// <summary>
        /// Executes the tool.
        /// </summary>
        /// <param name="invocation">The tool invocation.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The tool execution result.</returns>
        Task<ToolExecutionResult> ExecuteAsync(
            ToolInvocation invocation,
            ToolExecutionContext context,
            CancellationToken cancellationToken);
    }
}