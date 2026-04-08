using System.Text.Json;
using AiOperationsHub.Application.Abstractions.Audit;
using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Application.Abstractions.Providers;
using AiOperationsHub.Application.Abstractions.Security;
using AiOperationsHub.Application.Actions.Dtos;
using AiOperationsHub.Application.Common.Models;
using AiOperationsHub.Domain.Actions;
using AiOperationsHub.Domain.Ai;
using AiOperationsHub.Domain.Audit;
using MediatR;

namespace AiOperationsHub.Application.Actions.Commands.CreateJiraIssueProposal
{
    /// <summary>
    /// Handles creation of a Jira issue action proposal, including anonymization,
    /// provider-assisted preview generation, persistence, and audit recording.
    /// </summary>
    public sealed class CreateJiraIssueProposalCommandHandler : IRequestHandler<CreateJiraIssueProposalCommand, ActionProposalDto>
    {
        private readonly IActionProposalRepository _actionProposalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAnonymizationService _anonymizationService;
        private readonly IAiProvider _aiProvider;
        private readonly IAuditTrailWriter _auditTrailWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJiraIssueProposalCommandHandler"/> class.
        /// </summary>
        /// <param name="actionProposalRepository">The repository used to store action proposals.</param>
        /// <param name="unitOfWork">The unit-of-work used to persist changes atomically.</param>
        /// <param name="anonymizationService">The service used to sanitize sensitive outbound content.</param>
        /// <param name="aiProvider">The AI provider used to generate or refine proposal preview text.</param>
        /// <param name="auditTrailWriter">The audit writer used to record significant processing steps.</param>
        public CreateJiraIssueProposalCommandHandler(
            IActionProposalRepository actionProposalRepository,
            IUnitOfWork unitOfWork,
            IAnonymizationService anonymizationService,
            IAiProvider aiProvider,
            IAuditTrailWriter auditTrailWriter)
        {
            _actionProposalRepository = actionProposalRepository;
            _unitOfWork = unitOfWork;
            _anonymizationService = anonymizationService;
            _aiProvider = aiProvider;
            _auditTrailWriter = auditTrailWriter;
        }

        /// <summary>
        /// Handles the request to create a Jira issue proposal.
        /// </summary>
        /// <param name="request">The command containing Jira issue proposal input.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the created action proposal DTO.</returns>
        public async Task<ActionProposalDto> Handle(
            CreateJiraIssueProposalCommand request,
            CancellationToken cancellationToken)
        {
            await _auditTrailWriter.WriteAsync(
                AuditEventType.ActionProposed,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.RequestedByUserId,
                "Started creating Jira issue proposal.",
                "Application",
                request.EpicKey,
                null,
                cancellationToken);

            var rawPrompt =
                $"Create a Jira issue draft for project '{request.ProjectKey}' under epic '{request.EpicKey}'. " +
                $"Summary: '{request.Summary}'. Description: '{request.Description}'. " +
                $"Assignee: '{request.Assignee ?? "unassigned"}'.";

            var anonymizedPrompt = await _anonymizationService.AnonymizeAsync(rawPrompt, cancellationToken);

            await _auditTrailWriter.WriteAsync(
                AuditEventType.ContentAnonymized,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.RequestedByUserId,
                anonymizedPrompt.ContainsSensitiveData
                    ? "Sensitive data anonymized before provider call."
                    : "No sensitive data detected during anonymization.",
                "Anonymization",
                request.EpicKey,
                JsonSerializer.Serialize(new
                {
                    anonymizedPrompt.SanitizedText,
                    anonymizedPrompt.ContainsSensitiveData,
                    MappingCount = anonymizedPrompt.Mappings.Count
                }),
                cancellationToken);

            var providerResponse = await _aiProvider.GenerateAsync(
                new AiProviderRequest
                {
                    ProviderType = AiProviderType.OpenAi,
                    SystemPrompt =
                        "You are an assistant generating concise and professional Jira ticket previews. " +
                        "Do not include personal data; preserve placeholders.",
                    UserPrompt = anonymizedPrompt.SanitizedText,
                    CorrelationId = request.CorrelationId.ToString()
                },
                cancellationToken);

            await _auditTrailWriter.WriteAsync(
                AuditEventType.ProviderResponded,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.RequestedByUserId,
                "AI provider returned Jira issue proposal preview.",
                "OpenAI",
                request.EpicKey,
                providerResponse.RawResponseJson,
                cancellationToken);

            var actionParameters = new CreateJiraIssueActionParameters
            {
                ProjectKey = request.ProjectKey.Trim(),
                EpicKey = request.EpicKey.Trim(),
                Summary = request.Summary.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Assignee = string.IsNullOrWhiteSpace(request.Assignee)
                    ? null
                    : request.Assignee.Trim()
            };

            var parametersJson = JsonSerializer.Serialize(actionParameters);

            var previewText = string.IsNullOrWhiteSpace(providerResponse.OutputText)
                ? $"Create Jira issue in project '{actionParameters.ProjectKey}' under epic '{actionParameters.EpicKey}' with summary '{actionParameters.Summary}'."
                : providerResponse.OutputText.Trim();

            var proposal = ActionProposal.Create(
                request.RequestedByUserId,
                ActionTargetSystem.Jira,
                JiraActionType.CreateIssue.ToString(),
                request.EpicKey.Trim(),
                parametersJson,
                previewText,
                ActionRiskLevel.Medium);

            await _actionProposalRepository.AddAsync(proposal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditTrailWriter.WriteAsync(
                AuditEventType.ActionPreviewShown,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.RequestedByUserId,
                "Jira issue action proposal stored and ready for preview.",
                "Jira",
                proposal.TargetResource,
                JsonSerializer.Serialize(new
                {
                    proposal.Id,
                    proposal.ActionName,
                    proposal.Status,
                    proposal.PreviewText
                }),
                cancellationToken);

            return new ActionProposalDto
            {
                Id = proposal.Id,
                RequestedByUserId = request.RequestedByUserId,
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
                ExecutionResultJson = proposal.ExecutionResultJson,
                PromptTokenCount = providerResponse.PromptTokenCount,
                OutputTokenCount = providerResponse.OutputTokenCount,
                TotalTokenCount = providerResponse.TotalTokenCount,
                CumulativeTokenCount = providerResponse.CumulativeTokenCount,
                RequestPercentOfBudget = providerResponse.RequestPercentOfBudget,
                CumulativePercentOfBudget = providerResponse.CumulativePercentOfBudget
            };
        }
    }
}