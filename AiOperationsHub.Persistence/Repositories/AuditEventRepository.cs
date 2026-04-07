using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Domain.Audit;
using AiOperationsHub.Persistence.Db;
using AiOperationsHub.Persistence.Entities;

namespace AiOperationsHub.Persistence.Repositories
{
    /// <summary>
    /// Provides EF Core-backed persistence operations for audit events.
    /// </summary>
    public sealed class AuditEventRepository : IAuditEventRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditEventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context.</param>
        public AuditEventRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a new audit event to the database context.
        /// </summary>
        /// <param name="auditEvent">The audit event to add.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the audit event has been added to the change tracker.</returns>
        public Task AddAsync(AuditEvent auditEvent, CancellationToken cancellationToken)
        {
            var entity = new AuditEventDbEntity
            {
                Id = auditEvent.Id,
                CorrelationId = auditEvent.CorrelationId,
                ConversationId = auditEvent.ConversationId,
                UserId = auditEvent.UserId,
                EventType = (int)auditEvent.EventType,
                Verbosity = (int)auditEvent.Verbosity,
                SourceSystem = auditEvent.SourceSystem,
                TargetResource = auditEvent.TargetResource,
                Result = auditEvent.Result,
                PayloadJson = auditEvent.PayloadJson,
                CreatedAtUtc = auditEvent.CreatedAtUtc,
                UpdatedAtUtc = auditEvent.UpdatedAtUtc
            };

            _dbContext.AuditEvents.Add(entity);

            return Task.CompletedTask;
        }
    }
}