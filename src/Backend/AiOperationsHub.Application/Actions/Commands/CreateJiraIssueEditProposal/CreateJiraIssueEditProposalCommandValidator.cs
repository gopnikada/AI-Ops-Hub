namespace AiOperationsHub.Application.Actions.Commands.CreateJiraIssueEditProposal
{
    using System.Text.Json;
    using AiOperationsHub.Application.Abstractions.Audit;
    using AiOperationsHub.Application.Abstractions.Jira;
    using AiOperationsHub.Application.Abstractions.Persistence;
    using AiOperationsHub.Application.Abstractions.Resolution;
    using AiOperationsHub.Application.Actions.Dtos;
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;
    using AiOperationsHub.Domain.Audit;
    using AiOperationsHub.Domain.Common;
    using MediatR;

    /// <summary>
    /// Handles creation of Jira issue edit proposals.
    /// </summary>
    public sealed class CreateJiraIssueEditProposalCommandHandler
        : IRequestHandler<CreateJiraIssueEditProposalCommand, ProposalPreparationResultDto>
    {
        private readonly IActionProposalRepository _actionProposalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJiraConnector _jiraConnector;
        private readonly ITargetResourceResolver _targetResourceResolver;
        private readonly IAuditTrailWriter _auditTrailWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateJiraIssueEditProposalCommandHandler"/> class.
        /// </summary>
        /// <param name="actionProposalRepository">The proposal repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="jiraConnector">The Jira connector.</param>
        /// <param name="targetResourceResolver">The generic target resource resolver.</param>
        /// <param name="auditTrailWriter">The audit writer.</param>
        public CreateJiraIssueEditProposalCommandHandler(
            IActionProposalRepository actionProposalRepository,
            IUnitOfWork unitOfWork,
            IJiraConnector jiraConnector,
            ITargetResourceResolver targetResourceResolver,
            IAuditTrailWriter auditTrailWriter)
        {
            _actionProposalRepository = actionProposalRepository;
            _unitOfWork = unitOfWork;
            _jiraConnector = jiraConnector;
            _targetResourceResolver = targetResourceResolver;
            _auditTrailWriter = auditTrailWriter;
        }

        /// <inheritdoc />
        public async Task<ProposalPreparationResultDto> Handle(
            CreateJiraIssueEditProposalCommand request,
            CancellationToken cancellationToken)
        {
            await _auditTrailWriter.WriteAsync(
                AuditEventType.ActionProposed,
                AuditVerbosity.Standard,
                request.CorrelationId,
                request.ConversationId,
                request.RequestedByUserId,
                "Started creating Jira issue edit proposal.",
                "Jira",
                request.ResolvedIssueKey ?? request.IssueReference,
                null,
                cancellationToken);

            string issueKey;

            if (!string.IsNullOrWhiteSpace(request.ResolvedIssueKey))
            {
                issueKey = request.ResolvedIssueKey.Trim();
            }
            else
            {
                var resolution = await _targetResourceResolver.ResolveAsync(
                    new ResolveTargetResourceRequest
                    {
                        TargetSystem = ActionTargetSystem.Jira,
                        ScopeKey = request.ProjectKey,
                        Reference = request.IssueReference
                    },
                    cancellationToken);

                if (resolution.Status != TargetResourceResolutionStatus.SingleMatch)
                {
                    await _auditTrailWriter.WriteAsync(
                        AuditEventType.ActionPreviewShown,
                        AuditVerbosity.Standard,
                        request.CorrelationId,
                        request.ConversationId,
                        request.RequestedByUserId,
                        resolution.Status == TargetResourceResolutionStatus.MultipleMatches
                            ? "Jira issue edit proposal requires target selection before a proposal can be created."
                            : "No Jira issue matched the requested edit target.",
                        "Jira",
                        request.IssueReference,
                        JsonSerializer.Serialize(new
                        {
                            resolution.Status,
                            resolution.Reference,
                            resolution.ScopeKey,
                            resolution.ResolvedIdentifier,
                            Matches = resolution.Matches
                        }),
                        cancellationToken);

                    return new ProposalPreparationResultDto
                    {
                        Resolution = resolution
                    };
                }

                issueKey = resolution.ResolvedIdentifier!;
            }

            var currentIssue = await _jiraConnector.GetIssueAsync(
                issueKey,
                cancellationToken);

            var changes = BuildChangeSet(request, currentIssue);

            var parameters = new UpdateJiraIssueActionParameters
            {
                ProjectKey = request.ProjectKey,
                IssueReference = request.IssueReference,
                IssueKey = issueKey,
                Summary = request.Summary,
                Description = request.Description,
                Assignee = request.Assignee,
                Status = request.Status,
                ChangeSet = changes
            };

            var parametersJson = JsonSerializer.Serialize(parameters);

            var previewText = JiraIssueEditPreviewBuilder.Build(
                issueKey,
                currentIssue.Summary,
                changes);

            var proposal = ActionProposal.Create(
                request.RequestedByUserId,
                ActionTargetSystem.Jira,
                JiraActionType.EditIssue.ToString(),
                issueKey,
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
                "Jira issue edit proposal stored and ready for preview.",
                "Jira",
                issueKey,
                JsonSerializer.Serialize(new
                {
                    proposal.Id,
                    proposal.ActionName,
                    proposal.TargetResource,
                    parameters.ChangeSet
                }),
                cancellationToken);

            return new ProposalPreparationResultDto
            {
                Proposal = new ActionProposalDto
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
                }
            };
        }

        private static IReadOnlyCollection<JiraIssueFieldChange> BuildChangeSet(
            CreateJiraIssueEditProposalCommand request,
            JiraIssueDetailsResponse currentIssue)
        {
            var changes = new List<JiraIssueFieldChange>();

            if (!string.IsNullOrWhiteSpace(request.Summary) &&
                !string.Equals(request.Summary, currentIssue.Summary, StringComparison.Ordinal))
            {
                changes.Add(new JiraIssueFieldChange
                {
                    FieldName = "Summary",
                    CurrentValue = currentIssue.Summary,
                    ProposedValue = request.Summary
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Description) &&
                !string.Equals(request.Description, currentIssue.Description, StringComparison.Ordinal))
            {
                changes.Add(new JiraIssueFieldChange
                {
                    FieldName = "Description",
                    CurrentValue = currentIssue.Description,
                    ProposedValue = request.Description
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Assignee) &&
                !string.Equals(request.Assignee, currentIssue.Assignee, StringComparison.Ordinal))
            {
                changes.Add(new JiraIssueFieldChange
                {
                    FieldName = "Assignee",
                    CurrentValue = currentIssue.Assignee,
                    ProposedValue = request.Assignee
                });
            }

            if (!string.IsNullOrWhiteSpace(request.Status) &&
                !string.Equals(request.Status, currentIssue.Status, StringComparison.Ordinal))
            {
                changes.Add(new JiraIssueFieldChange
                {
                    FieldName = "Status",
                    CurrentValue = currentIssue.Status,
                    ProposedValue = request.Status
                });
            }

            if (changes.Count == 0)
            {
                throw new DomainException("No effective Jira issue changes were detected.");
            }

            return changes;
        }
    }
}