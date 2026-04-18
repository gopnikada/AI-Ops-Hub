namespace AiOperationsHub.Application.Actions.Execution
{
    /// <summary>
    /// Represents the outcome of executing an action proposal.
    /// </summary>
    public sealed class ActionProposalExecutionResult
    {
        /// <summary>
        /// Gets or sets the logical resource identifier created or affected by execution.
        /// </summary>
        public string? ResourceId { get; set; }

        /// <summary>
        /// Gets or sets the serialized execution result payload.
        /// </summary>
        public string ExecutionResultJson { get; set; } = null!;
    }
}