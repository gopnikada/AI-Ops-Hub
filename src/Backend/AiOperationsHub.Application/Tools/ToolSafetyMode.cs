namespace AiOperationsHub.Application.Tools
{
    /// <summary>
    /// Describes the safety mode of a tool.
    /// </summary>
    public enum ToolSafetyMode
    {
        /// <summary>
        /// Indicates that the tool is read-only.
        /// </summary>
        ReadOnly = 1,

        /// <summary>
        /// Indicates that the tool requires explicit confirmation before real side effects occur.
        /// </summary>
        RequiresConfirmation = 2
    }
}