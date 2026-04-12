namespace AiOperationsHub.Api.Contracts.Chat
{
    /// <summary>
    /// Represents the incoming API contract for a streamed chat request.
    /// </summary>
    public sealed class ChatStreamRequestContract
    {
        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the plain-language message.
        /// </summary>
        public string Message { get; set; } = null!;
    }
}