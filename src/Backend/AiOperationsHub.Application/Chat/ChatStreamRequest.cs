namespace AiOperationsHub.Application.Chat
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents an incoming plain-language chat request.
    /// </summary>
    public sealed class ChatStreamRequest
    {
        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the user message.
        /// </summary>
        [Required]
        public string Message { get; set; } = null!;
    }
}