namespace AiOperationsHub.Infrastructure.Tools
{
    using AiOperationsHub.Application.Tools;

    /// <summary>
    /// Default in-process tool registry.
    /// </summary>
    public sealed class ToolRegistry : IToolRegistry
    {
        private readonly IReadOnlyDictionary<string, IChatTool> _tools;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolRegistry"/> class.
        /// </summary>
        /// <param name="tools">The registered tools.</param>
        public ToolRegistry(IEnumerable<IChatTool> tools)
        {
            _tools = tools.ToDictionary(
                x => x.Definition.Name,
                StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ToolDefinition> GetDefinitions()
        {
            return _tools.Values
                .Select(x => x.Definition)
                .ToArray();
        }

        /// <inheritdoc />
        public IChatTool? Find(string toolName)
        {
            _tools.TryGetValue(toolName, out var tool);
            return tool;
        }
    }
}