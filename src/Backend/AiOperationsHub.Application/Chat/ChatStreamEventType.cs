namespace AiOperationsHub.Application.Chat
{
    /// <summary>
    /// Defines event types emitted by the chat streaming workflow.
    /// </summary>
    public static class ChatStreamEventType
    {
        /// <summary>
        /// Indicates that the request has been received.
        /// </summary>
        public const string MessageReceived = "message.received";

        /// <summary>
        /// Indicates that AI analysis has started.
        /// </summary>
        public const string AnalysisStarted = "analysis.started";

        /// <summary>
        /// Indicates that a tool was selected.
        /// </summary>
        public const string ToolSelected = "tool.selected";

        /// <summary>
        /// Indicates that a proposal has been created.
        /// </summary>
        public const string ProposalReady = "proposal.ready";

        /// <summary>
        /// Indicates that user confirmation is required.
        /// </summary>
        public const string ConfirmationRequired = "confirmation.required";

        /// <summary>
        /// Indicates that a direct assistant message is available.
        /// </summary>
        public const string AssistantMessage = "assistant.message";

        /// <summary>
        /// Indicates that an error occurred.
        /// </summary>
        public const string Error = "error";
    }
}