namespace AiOperationsHub.Application.Actions.Commands.ConfirmActionProposal
{
    using System.Text.Json;
    using AiOperationsHub.Application.Abstractions.Actions;
    using AiOperationsHub.Application.Abstractions.Audit;
    using AiOperationsHub.Application.Abstractions.Persistence;
    using AiOperationsHub.Application.Actions.Dtos;
    using AiOperationsHub.Domain.Audit;
    using AiOperationsHub.Domain.Common;
    using MediatR;

    /// <summary>
    /// Handles confirmation and execution of an existing action proposal.
    /// </summary>
    public sealed class ConfirmActionProposalCommandHandler : IRequestHandler<ConfirmActionProposalCommand, ActionProposalDto>
    {
        private readonly IActionProposalRepository _actionProposalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IActionProposalExecutionDispatcher _executionDispatcher;
        private readonly IAuditTrailWriter _auditTrailWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmActionProposalCommandHandler"/> class.
        /// </summary>
        /// <param name="actionProposalRepository">The repository used to retrieve action proposals.</param>
        /// <param name="unitOfWork">The unit-of-work used to persist proposal state changes.</param>
        /// <param name="executionDispatcher">The dispatcher used to execute proposals through registered executors.</param>
        /// <param name="auditTrailWriter">The audit writer used to record proposal lifecycle events.</param>
        public ConfirmActionProposalCommandHandler(
            IActionProposalRepository actionProposalRepository,
            IUnitOfWork unitOfWork,
            IActionProposalExecutionDispatcher executionDispatcher,
            IAuditTrailWriter auditTrailWriter)
        {
            _actionProposalRepository = actionProposalRepository;
            _unitOfWork = unitOfWork;
            _executionDispatcher = executionDispatcher;
            _auditTrailWriter = auditTrailWriter;
        }

        /// <inheritdoc />
        public async Task<ActionProposalDto> Handle(
            ConfirmActionProposalCommand request,
            CancellationToken cancellationToken)
        {
            var proposal = await _actionProposalRepository.GetByIdAsync(request.ProposalId, cancellationToken);

            if (proposal is null)
            {
                throw new DomainException($"Action proposal '{request.ProposalId}' was not found.");
            }

            proposal.Confirm(DateTime.UtcNow);

            await _auditTrailWriter.WriteAsync(
                AuditEventType.ActionConfirmationReceived,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.ConfirmedByUserId,
                "Action proposal confirmation received.",
                proposal.TargetSystem.ToString(),
                proposal.TargetResource,
                JsonSerializer.Serialize(new
                {
                    proposal.Id,
                    proposal.ActionName,
                    proposal.Status,
                    proposal.ConfirmedAtUtc
                }),
                cancellationToken);

            proposal.StartExecution();

            await _auditTrailWriter.WriteAsync(
                AuditEventType.ActionExecutionStarted,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.ConfirmedByUserId,
                "Action proposal execution started.",
                proposal.TargetSystem.ToString(),
                proposal.TargetResource,
                JsonSerializer.Serialize(new
                {
                    proposal.Id,
                    proposal.ActionName,
                    proposal.Status
                }),
                cancellationToken);

            try
            {
                var executionResult = await _executionDispatcher.ExecuteAsync(
                    proposal,
                    cancellationToken);

                proposal.MarkExecuted(
                    DateTime.UtcNow,
                    executionResult.ExecutionResultJson);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _auditTrailWriter.WriteAsync(
                    AuditEventType.ActionExecutionSucceeded,
                    AuditVerbosity.Standard,
                    request.CorrelationId,
                    request.ConversationId,
                    request.ConfirmedByUserId,
                    "Action proposal executed successfully.",
                    proposal.TargetSystem.ToString(),
                    executionResult.ResourceId ?? proposal.TargetResource,
                    executionResult.ExecutionResultJson,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                proposal.MarkFailed(JsonSerializer.Serialize(new
                {
                    Error = ex.Message
                }));

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _auditTrailWriter.WriteAsync(
                    AuditEventType.ActionExecutionFailed,
                    AuditVerbosity.Standard,
                    request.CorrelationId,
                    request.ConversationId,
                    request.ConfirmedByUserId,
                    "Action proposal execution failed.",
                    proposal.TargetSystem.ToString(),
                    proposal.TargetResource,
                    JsonSerializer.Serialize(new
                    {
                        ExceptionMessage = ex.Message
                    }),
                    cancellationToken);

                throw;
            }

            return new ActionProposalDto
            {
                Id = proposal.Id,
                RequestedByUserId = proposal.RequestedByUserId,
                TargetSystem = proposal.TargetSystem,
                ActionName = proposal.ActionName,
                TargetResource = proposal.TargetResource,
                ParametersJson = proposal.ParametersJson,
                PreviewText = proposal.PreviewText,
                RiskLevel = proposal.RiskLevel,
                Status = proposal.Status,
                CreatedAtUtc = proposal.CreatedAtUtc,
                ConfirmedAtUtc = proposal.ConfirmedAtUtc,
                ExecutedAtUtc = proposal.ExecutedAtUtc,
                ExecutionResultJson = proposal.ExecutionResultJson
            };
        }
    }
}