using AiOperationsHub.Domain.Common;

namespace AiOperationsHub.Domain.Audit
{
    public sealed class AuditEvent : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public Guid? ConversationId { get; set; }
        public Guid? UserId { get; set; }
        public AuditEventType EventType { get; set; }
        public AuditVerbosity Verbosity { get; set; }
        public string? SourceSystem { get; set; }
        public string? TargetResource { get; set; }
        public string Result { get; set; } = null!;
        public string? PayloadJson { get; set; }
    }
}
