using AiOperationsHub.Domain.Audit;

namespace AiOperationsHub.Application.Abstractions.Persistence
{
    /// <summary>
    /// Provides persistence operations for audit events.
    /// </summary>
    public interface IAuditEventRepository
    {
        /// <summary>
        /// Adds a new audit event to the persistence store.
        /// </summary>
        /// <param name="auditEvent">The audit event to add.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the event has been staged for persistence.</returns>
        Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken);
    }
}