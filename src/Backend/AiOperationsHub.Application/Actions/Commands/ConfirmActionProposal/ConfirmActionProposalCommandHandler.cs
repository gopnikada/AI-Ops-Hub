using System.Text.Json;
using AiOperationsHub.Application.Abstractions.Audit;
using AiOperationsHub.Application.Abstractions.Jira;
using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Application.Actions.Dtos;
using AiOperationsHub.Application.Common.Models;
using AiOperationsHub.Domain.Actions;
using AiOperationsHub.Domain.Audit;
using AiOperationsHub.Domain.Common;
using MediatR;

namespace AiOperationsHub.Application.Actions.Commands.ConfirmActionProposal
{
    /// <summary>
    /// Handles confirmation and execution of an existing action proposal.
    /// </summary>
    public sealed class ConfirmActionProposalCommandHandler : IRequestHandler<ConfirmActionProposalCommand, ActionProposalDto>
    {
        private readonly IActionProposalRepository _actionProposalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJiraConnector _jiraConnector;
        private readonly IAuditTrailWriter _auditTrailWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmActionProposalCommandHandler"/> class.
        /// </summary>
        /// <param name="actionProposalRepository">The repository used to retrieve action proposals.</param>
        /// <param name="unitOfWork">The unit-of-work used to persist proposal state changes.</param>
        /// <param name="jiraConnector">The Jira connector used to execute Jira actions.</param>
        /// <param name="auditTrailWriter">The audit writer used to record proposal lifecycle events.</param>
        public ConfirmActionProposalCommandHandler(
            IActionProposalRepository actionProposalRepository,
            IUnitOfWork unitOfWork,
            IJiraConnector jiraConnector,
            IAuditTrailWriter auditTrailWriter)
        {
            _actionProposalRepository = actionProposalRepository;
            _unitOfWork = unitOfWork;
            _jiraConnector = jiraConnector;
            _auditTrailWriter = auditTrailWriter;
        }

        /// <summary>
        /// Handles the request to confirm and execute an action proposal.
        /// </summary>
        /// <param name="request">The command containing proposal confirmation input.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the updated action proposal DTO.</returns>
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

            if (proposal.TargetSystem != ActionTargetSystem.Jira ||
                !string.Equals(proposal.ActionName, JiraActionType.CreateIssue.ToString(), StringComparison.Ordinal))
            {
                throw new DomainException("Unsupported action proposal target system or action name.");
            }

            var parameters = JsonSerializer.Deserialize<CreateJiraIssueActionParameters>(proposal.ParametersJson);

            if (parameters is null)
            {
                throw new DomainException("Action proposal parameters are invalid.");
            }

            try
            {
                var jiraResult = await _jiraConnector.CreateIssueAsync(
                    new CreateJiraIssueDraftRequest
                    {
                        ProjectKey = parameters.ProjectKey,
                        EpicKey = parameters.EpicKey,
                        Summary = parameters.Summary,
                        Description = parameters.Description,
                        Assignee = parameters.Assignee
                    },
                    cancellationToken);

                proposal.MarkExecuted(
                    DateTime.UtcNow,
                    JsonSerializer.Serialize(new
                    {
                        jiraResult.IssueKey,
                        jiraResult.IssueUrl,
                        jiraResult.RawResponseJson
                    }));

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _auditTrailWriter.WriteAsync(
                    AuditEventType.ActionExecutionSucceeded,
                    AuditVerbosity.Standard,
                    request.CorrelationId,
                    request.ConversationId,
                    request.ConfirmedByUserId,
                    "Jira issue created successfully.",
                    "Jira",
                    jiraResult.IssueKey,
                    JsonSerializer.Serialize(jiraResult),
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
                    "Jira issue creation failed.",
                    "Jira",
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