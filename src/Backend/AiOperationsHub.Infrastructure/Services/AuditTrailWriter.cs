using AiOperationsHub.Application.Abstractions.Audit;
using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Domain.Audit;
using Microsoft.Extensions.Logging;

namespace AiOperationsHub.Infrastructure.Services
{
    /// <summary>
    /// Writes normalized audit trail entries to the persistence store.
    /// </summary>
    public sealed class AuditTrailWriter : IAuditTrailWriter
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuditTrailWriter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditTrailWriter"/> class.
        /// </summary>
        /// <param name="auditEventRepository">The audit event repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="logger">The logger.</param>
        public AuditTrailWriter(
            IAuditEventRepository auditEventRepository,
            IUnitOfWork unitOfWork,
            ILogger<AuditTrailWriter> logger)
        {
            _auditEventRepository = auditEventRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

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
        public async Task WriteAsync(
            AuditEventType eventType,
            AuditVerbosity verbosity,
            Guid correlationId,
            Guid? conversationId,
            Guid? userId,
            string result,
            string? sourceSystem,
            string? targetResource,
            string? payloadJson,
            CancellationToken cancellationToken)
        {
            var auditEvent = new AuditEvent
            {
                Id = Guid.NewGuid(),
                CorrelationId = correlationId,
                ConversationId = conversationId,
                UserId = userId,
                EventType = eventType,
                Verbosity = verbosity,
                SourceSystem = sourceSystem,
                TargetResource = targetResource,
                Result = result,
                PayloadJson = payloadJson
            };

            await _auditEventRepository.AddAsync(auditEvent, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Audit event persisted. EventType: {EventType}, CorrelationId: {CorrelationId}, UserId: {UserId}",
                eventType,
                correlationId,
                userId);
        }
    }
}