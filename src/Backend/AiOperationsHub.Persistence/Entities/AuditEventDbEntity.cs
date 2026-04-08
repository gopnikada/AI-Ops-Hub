namespace AiOperationsHub.Persistence.Entities
{
    /// <summary>
    /// Represents the persisted database record for an audit event.
    /// </summary>
    public sealed class AuditEventDbEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the audit event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the correlation identifier used for request tracing.
        /// </summary>
        public Guid CorrelationId { get; set; }

        /// <summary>
        /// Gets or sets the optional conversation identifier associated with the event.
        /// </summary>
        public Guid? ConversationId { get; set; }

        /// <summary>
        /// Gets or sets the optional user identifier associated with the event.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the audit event type enum value.
        /// </summary>
        public int EventType { get; set; }

        /// <summary>
        /// Gets or sets the verbosity enum value used for the event.
        /// </summary>
        public int Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the originating source system or subsystem.
        /// </summary>
        public string? SourceSystem { get; set; }

        /// <summary>
        /// Gets or sets the target resource associated with the event.
        /// </summary>
        public string? TargetResource { get; set; }

        /// <summary>
        /// Gets or sets the result or outcome description.
        /// </summary>
        public string Result { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional serialized payload associated with the event.
        /// </summary>
        public string? PayloadJson { get; set; }

        /// <summary>
        /// Gets or sets the UTC creation timestamp.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC update timestamp.
        /// </summary>
        public DateTime? UpdatedAtUtc { get; set; }
    }
}