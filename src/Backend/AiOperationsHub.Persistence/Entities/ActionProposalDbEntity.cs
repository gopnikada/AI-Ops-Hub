namespace AiOperationsHub.Persistence.Entities
{
    /// <summary>
    /// Represents the persisted database record for an action proposal.
    /// </summary>
    public sealed class ActionProposalDbEntity
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
        /// Gets or sets the target system enum value.
        /// </summary>
        public int TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the normalized action name for the target system.
        /// </summary>
        public string ActionName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the logical target resource associated with the proposal.
        /// </summary>
        public string TargetResource { get; set; } = null!;

        /// <summary>
        /// Gets or sets the serialized action parameters payload.
        /// </summary>
        public string ParametersJson { get; set; } = null!;

        /// <summary>
        /// Gets or sets the preview text shown before confirmation.
        /// </summary>
        public string PreviewText { get; set; } = null!;

        /// <summary>
        /// Gets or sets the risk level enum value.
        /// </summary>
        public int RiskLevel { get; set; }

        /// <summary>
        /// Gets or sets the proposal status enum value.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets the UTC creation timestamp.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC update timestamp.
        /// </summary>
        public DateTime? UpdatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC confirmation timestamp.
        /// </summary>
        public DateTime? ConfirmedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC execution timestamp.
        /// </summary>
        public DateTime? ExecutedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the serialized execution result payload.
        /// </summary>
        public string? ExecutionResultJson { get; set; }
    }
}