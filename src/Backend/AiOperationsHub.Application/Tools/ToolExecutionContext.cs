namespace AiOperationsHub.Application.Tools
{
    /// <summary>
    /// Provides execution context for a tool invocation.
    /// </summary>
    public sealed class ToolExecutionContext
    {
        /// <summary>
        /// Gets or sets the requesting user identifier.
        /// </summary>
        public Guid RequestedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the original user message.
        /// </summary>
        public string OriginalMessage { get; set; } = null!;
    }
}