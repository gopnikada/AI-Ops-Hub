using AiOperationsHub.Domain.Actions;

namespace AiOperationsHub.Application.Actions.Dtos
{
    /// <summary>
    /// Represents a normalized action proposal DTO returned by application use cases.
    /// </summary>
    public sealed class ActionProposalDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the proposal.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who requested the proposal.
        /// </summary>
        public Guid RequestedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the target system against which the action will be performed.
        /// </summary>
        public ActionTargetSystem TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the normalized action name for the target system.
        /// </summary>
        public string ActionName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the logical target resource associated with the proposal.
        /// </summary>
        public string TargetResource { get; set; } = null!;

        /// <summary>
        /// Gets or sets the serialized action parameter payload.
        /// </summary>
        public string ParametersJson { get; set; } = null!;

        /// <summary>
        /// Gets or sets the preview text shown to the user before confirmation.
        /// </summary>
        public string PreviewText { get; set; } = null!;

        /// <summary>
        /// Gets or sets the assessed risk level of the proposal.
        /// </summary>
        public ActionRiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Gets or sets the current lifecycle status of the proposal.
        /// </summary>
        public ActionProposalStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the proposal was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the proposal was confirmed, when available.
        /// </summary>
        public DateTime? ConfirmedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when the proposal was executed, when available.
        /// </summary>
        public DateTime? ExecutedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the serialized execution result payload, when available.
        /// </summary>
        public string? ExecutionResultJson { get; set; }

        /// <summary>
        /// Gets or sets the prompt token count reported by the AI provider.
        /// </summary>
        public int? PromptTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the output token count reported by the AI provider.
        /// </summary>
        public int? OutputTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the total token count reported by the AI provider.
        /// </summary>
        public int? TotalTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the cumulative token count tracked by the application.
        /// </summary>
        public long? CumulativeTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the percentage of the configured budget used by this request.
        /// </summary>
        public decimal? RequestPercentOfBudget { get; set; }

        /// <summary>
        /// Gets or sets the percentage of the configured budget used cumulatively.
        /// </summary>
        public decimal? CumulativePercentOfBudget { get; set; }
    }
}