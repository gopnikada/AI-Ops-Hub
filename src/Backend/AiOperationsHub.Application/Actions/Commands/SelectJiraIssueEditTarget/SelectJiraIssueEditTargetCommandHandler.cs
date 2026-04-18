namespace AiOperationsHub.Application.Actions.Commands.SelectJiraIssueEditTarget
{
    using AiOperationsHub.Application.Abstractions.Audit;
    using AiOperationsHub.Application.Abstractions.Jira;
    using AiOperationsHub.Application.Abstractions.Persistence;
    using AiOperationsHub.Application.Actions.Commands.CreateJiraIssueEditProposal;
    using AiOperationsHub.Application.Actions.Dtos;
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;
    using AiOperationsHub.Domain.Audit;
    using AiOperationsHub.Domain.Common;
    using MediatR;
    using System.Text.Json;

    /// <summary>
    /// Handles creation of Jira issue edit proposals after a concrete Jira issue has been selected.
    /// </summary>
    public sealed class SelectJiraIssueEditTargetCommandHandler
        : IRequestHandler<SelectJiraIssueEditTargetCommand, ActionProposalDto>
    {
        private readonly IActionProposalRepository _actionProposalRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJiraConnector _jiraConnector;
        private readonly IAuditTrailWriter _auditTrailWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectJiraIssueEditTargetCommandHandler"/> class.
        /// </summary>
        /// <param name="actionProposalRepository">The proposal repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="jiraConnector">The Jira connector.</param>
        /// <param name="auditTrailWriter">The audit writer.</param>
        public SelectJiraIssueEditTargetCommandHandler(
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

        /// <inheritdoc />
        public async Task<ActionProposalDto> Handle(
            SelectJiraIssueEditTargetCommand request,
            CancellationToken cancellationToken)
        {
            var currentIssue = await _jiraConnector.GetIssueAsync(
                request.SelectedIssueKey,
                cancellationToken);

            var changes = BuildChangeSet(request, currentIssue);

            var parameters = new UpdateJiraIssueActionParameters
            {
                ProjectKey = request.ProjectKey,
                IssueReference = request.IssueReference,
                IssueKey = request.SelectedIssueKey,
                Summary = request.Summary,
                Description = request.Description,
                Assignee = request.Assignee,
                Status = request.Status,
                ChangeSet = changes
            };

            var parametersJson = JsonSerializer.Serialize(parameters);

            var previewText = JiraIssueEditPreviewBuilder.Build(
                request.SelectedIssueKey,
                currentIssue.Summary,
                changes);

            var proposal = ActionProposal.Create(
                request.RequestedByUserId,
                ActionTargetSystem.Jira,
                JiraActionType.EditIssue.ToString(),
                request.SelectedIssueKey,
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
                "Jira issue edit proposal created after candidate selection.",
                "Jira",
                request.SelectedIssueKey,
                JsonSerializer.Serialize(new
                {
                    proposal.Id,
                    proposal.ActionName,
                    proposal.TargetResource,
                    parameters.ChangeSet
                }),
                cancellationToken);

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

        private static IReadOnlyCollection<JiraIssueFieldChange> BuildChangeSet(
            SelectJiraIssueEditTargetCommand request,
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