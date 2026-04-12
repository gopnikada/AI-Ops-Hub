namespace AiOperationsHub.Application.Chat
{
    /// <summary>
    /// Orchestrates AI-backed chat requests and streams structured backend events.
    /// </summary>
    public interface IChatOrchestrator
    {
        /// <summary>
        /// Processes a chat request and returns a stream of backend events.
        /// </summary>
        /// <param name="request">The chat request.</param>
        /// <param name="requestedByUserId">The user identifier.</param>
        /// <param name="correlationId">The correlation identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An async stream of chat events.</returns>
        IAsyncEnumerable<ChatStreamEvent> StreamAsync(
            ChatStreamRequest request,
            Guid requestedByUserId,
            Guid correlationId,
            CancellationToken cancellationToken);
    }
}