namespace AiOperationsHub.Api.Contracts.Requests
{
    /// <summary>
    /// Represents the HTTP request body for confirming an action proposal.
    /// </summary>
    public sealed class ConfirmActionProposalRequest
    {
        /// <summary>
        /// Gets or sets the optional conversation identifier.
        /// </summary>
        public Guid? ConversationId { get; set; }
    }
}