namespace AiOperationsHub.Application.Tools
{
    /// <summary>
    /// Exposes the set of tools available to the chat orchestration layer.
    /// </summary>
    public interface IToolRegistry
    {
        /// <summary>
        /// Gets all available tool definitions.
        /// </summary>
        /// <returns>The available tool definitions.</returns>
        IReadOnlyCollection<ToolDefinition> GetDefinitions();

        /// <summary>
        /// Finds a tool by name.
        /// </summary>
        /// <param name="toolName">The tool name.</param>
        /// <returns>The resolved tool, or null.</returns>
        IChatTool? Find(string toolName);
    }
}