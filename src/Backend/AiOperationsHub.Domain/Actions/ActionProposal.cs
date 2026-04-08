using AiOperationsHub.Domain.Common;

namespace AiOperationsHub.Domain.Actions
{

    public sealed class ActionProposal : AuditableEntity
    {
        public Guid Id { get; private set; }
        public Guid RequestedByUserId { get; private set; }
        public ActionTargetSystem TargetSystem { get; private set; }
        public string ActionName { get; private set; }
        public string TargetResource { get; private set; }
        public string ParametersJson { get; private set; }
        public string PreviewText { get; private set; }
        public ActionRiskLevel RiskLevel { get; private set; }
        public ActionProposalStatus Status { get; private set; }
        public DateTime? ConfirmedAtUtc { get; private set; }
        public DateTime? ExecutedAtUtc { get; private set; }
        public string? ExecutionResultJson { get; private set; }

        private ActionProposal(
            Guid id,
            Guid requestedByUserId,
            ActionTargetSystem targetSystem,
            string actionName,
            string targetResource,
            string parametersJson,
            string previewText,
            ActionRiskLevel riskLevel,
            ActionProposalStatus status)
        {
            Id = id;
            RequestedByUserId = requestedByUserId;
            TargetSystem = targetSystem;
            ActionName = actionName;
            TargetResource = targetResource;
            ParametersJson = parametersJson;
            PreviewText = previewText;
            RiskLevel = riskLevel;
            Status = status;
        }

        public static ActionProposal Create(
            Guid requestedByUserId,
            ActionTargetSystem targetSystem,
            string actionName,
            string targetResource,
            string parametersJson,
            string previewText,
            ActionRiskLevel riskLevel)
        {
            DomainValidation.RequireTrue(requestedByUserId != Guid.Empty, "RequestedByUserId is required.");
            DomainValidation.RequireNotNullOrWhiteSpace(actionName, nameof(actionName));
            DomainValidation.RequireNotNullOrWhiteSpace(targetResource, nameof(targetResource));
            DomainValidation.RequireNotNullOrWhiteSpace(parametersJson, nameof(parametersJson));
            DomainValidation.RequireNotNullOrWhiteSpace(previewText, nameof(previewText));
            DomainValidation.RequireMaxLength(previewText, 4000, nameof(previewText));

            return new ActionProposal(
                Guid.NewGuid(),
                requestedByUserId,
                targetSystem,
                actionName.Trim(),
                targetResource.Trim(),
                parametersJson,
                previewText.Trim(),
                riskLevel,
                ActionProposalStatus.AwaitingConfirmation);
        }

        public void Confirm(DateTime confirmedAtUtc)
        {
            DomainValidation.RequireTrue(
                Status == ActionProposalStatus.AwaitingConfirmation,
                "Only proposals awaiting confirmation can be confirmed.");

            Status = ActionProposalStatus.Confirmed;
            ConfirmedAtUtc = confirmedAtUtc;
            Touch();
        }

        public void StartExecution()
        {
            DomainValidation.RequireTrue(
                Status == ActionProposalStatus.Confirmed,
                "Only confirmed proposals can start execution.");

            Status = ActionProposalStatus.Executing;
            Touch();
        }

        public void MarkExecuted(DateTime executedAtUtc, string? executionResultJson)
        {
            DomainValidation.RequireTrue(
                Status == ActionProposalStatus.Executing,
                "Only executing proposals can be marked as executed.");

            Status = ActionProposalStatus.Executed;
            ExecutedAtUtc = executedAtUtc;
            ExecutionResultJson = executionResultJson;
            Touch();
        }

        public void MarkFailed(string? executionResultJson)
        {
            DomainValidation.RequireTrue(
                Status == ActionProposalStatus.Executing,
                "Only executing proposals can be marked as failed.");

            Status = ActionProposalStatus.Failed;
            ExecutionResultJson = executionResultJson;
            Touch();
        }

        public void Reject()
        {
            DomainValidation.RequireTrue(
                Status == ActionProposalStatus.AwaitingConfirmation,
                "Only proposals awaiting confirmation can be rejected.");

            Status = ActionProposalStatus.Rejected;
            Touch();
        }
        public static ActionProposal Rehydrate(
            Guid id,
            Guid requestedByUserId,
            ActionTargetSystem targetSystem,
            string actionName,
            string targetResource,
            string parametersJson,
            string previewText,
            ActionRiskLevel riskLevel,
            ActionProposalStatus status,
            DateTime createdAtUtc,
            DateTime? updatedAtUtc,
            DateTime? confirmedAtUtc,
            DateTime? executedAtUtc,
            string? executionResultJson)
        {
            var proposal = new ActionProposal(
                id,
                requestedByUserId,
                targetSystem,
                actionName,
                targetResource,
                parametersJson,
                previewText,
                riskLevel,
                status);

            proposal.CreatedAtUtc = createdAtUtc;
            proposal.UpdatedAtUtc = updatedAtUtc;
            proposal.ConfirmedAtUtc = confirmedAtUtc;
            proposal.ExecutedAtUtc = executedAtUtc;
            proposal.ExecutionResultJson = executionResultJson;

            return proposal;
        }
    }
}
