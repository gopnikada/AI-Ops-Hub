using AiOperationsHub.Domain.Audit;

namespace AiOperationsHub.Application.Abstractions.Audit
{
    /// <summary>
    /// Writes structured audit trail entries for significant application events.
    /// </summary>
    public interface IAuditTrailWriter
    {
        /// <summary>
        /// Writes a single audit event using normalized audit metadata.
        /// </summary>
        /// <param name="eventType">The type of audit event being recorded.</param>
        /// <param name="verbosity">The verbosity profile used for the event payload.</param>
        /// <param name="correlationId">The correlation identifier for tracing the request flow.</param>
        /// <param name="conversationId">The optional conversation identifier associated with the event.</param>
        /// <param name="userId">The optional user identifier associated with the event.</param>
        /// <param name="result">A human-readable result or outcome description.</param>
        /// <param name="sourceSystem">The source system or subsystem producing the event.</param>
        /// <param name="targetResource">The logical resource or target object associated with the event.</param>
        /// <param name="payloadJson">Optional serialized payload details for the event.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the audit event has been written.</returns>
        Task WriteAsync(
            AuditEventType eventType,
            AuditVerbosity verbosity,
            Guid correlationId,
            Guid? conversationId,
            Guid? userId,
            string result,
            string? sourceSystem,
            string? targetResource,
            string? payloadJson,
            CancellationToken cancellationToken);
    }
}