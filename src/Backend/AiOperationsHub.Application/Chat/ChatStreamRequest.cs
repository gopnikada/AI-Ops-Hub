namespace AiOperationsHub.Application.Chat
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an incoming plain-language chat request.
    /// </summary>
    public sealed class ChatStreamRequest
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
        /// Gets or sets the user message to process.
        /// </summary>
        [Required]
        public string Message { get; set; } = null!;
    }
}