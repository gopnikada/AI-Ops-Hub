namespace AiOperationsHub.Application.Chat
{
    using AiOperationsHub.Application.Actions.Dtos;

    /// <summary>
    /// Represents one streamed event emitted by the backend chat workflow.
    /// </summary>
    public sealed class ChatStreamEvent
    {
        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional message.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Gets or sets the optional selected tool name.
        /// </summary>
        public string? ToolName { get; set; }

        /// <summary>
        /// Gets or sets the optional proposal identifier.
        /// </summary>
        public Guid? ProposalId { get; set; }

        /// <summary>
        /// Gets or sets the optional proposal payload.
        /// </summary>
        public ActionProposalDto? Proposal { get; set; }

        /// <summary>
        /// Gets or sets the optional raw JSON data for the frontend.
        /// </summary>
        public string? DataJson { get; set; }

        /// <summary>
        /// Creates a simple event instance.
        /// </summary>
        /// <param name="type">The event type.</param>
        /// <param name="message">The optional message.</param>
        /// <returns>The created event.</returns>
        public static ChatStreamEvent Create(string type, string? message = null)
        {
            return new ChatStreamEvent
            {
                Type = type,
                Message = message
            };
        }
    }
}