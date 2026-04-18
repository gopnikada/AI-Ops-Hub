namespace AiOperationsHub.Api.Contracts.Chat
{
    /// <summary>
    /// Represents the incoming API contract for a streamed chat request.
    /// </summary>
    public sealed class ChatStreamRequestContract
    {
        /// <summary>
        /// Gets or sets the optional conversation identifier associated with the request.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the optional proposal identifier referenced by the chat message.
        /// </summary>
        public Guid? ProposalId { get; set; }

        /// <summary>
        /// Gets or sets the plain-language user message.
        /// </summary>
        public string Message { get; set; } = null!;
    }
}