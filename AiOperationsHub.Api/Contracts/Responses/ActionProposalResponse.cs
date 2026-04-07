using AiOperationsHub.Application.Actions.Dtos;
using AiOperationsHub.Domain.Actions;

namespace AiOperationsHub.Api.Contracts.Responses
{
    /// <summary>
    /// Represents the HTTP response contract for an action proposal.
    /// </summary>
    public sealed class ActionProposalResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the proposal.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user who requested the proposal.
        /// </summary>
        public Guid RequestedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the target system.
        /// </summary>
        public ActionTargetSystem TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        public string ActionName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the target resource.
        /// </summary>
        public string TargetResource { get; set; } = null!;

        /// <summary>
        /// Gets or sets the serialized parameters JSON.
        /// </summary>
        public string ParametersJson { get; set; } = null!;

        /// <summary>
        /// Gets or sets the preview text.
        /// </summary>
        public string PreviewText { get; set; } = null!;

        /// <summary>
        /// Gets or sets the risk level.
        /// </summary>
        public ActionRiskLevel RiskLevel { get; set; }

        /// <summary>
        /// Gets or sets the proposal status.
        /// </summary>
        public ActionProposalStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the creation timestamp.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the confirmation timestamp.
        /// </summary>
        public DateTime? ConfirmedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the execution timestamp.
        /// </summary>
        public DateTime? ExecutedAtUtc { get; set; }

        /// <summary>
        /// Gets or sets the execution result JSON.
        /// </summary>
        public string? ExecutionResultJson { get; set; }

        /// <summary>
        /// Maps an application DTO to an API response contract.
        /// </summary>
        /// <param name="dto">The source DTO.</param>
        /// <returns>The mapped response.</returns>
        public static ActionProposalResponse FromDto(ActionProposalDto dto)
        {
            return new ActionProposalResponse
            {
                Id = dto.Id,
                RequestedByUserId = dto.RequestedByUserId,
                TargetSystem = dto.TargetSystem,
                ActionName = dto.ActionName,
                TargetResource = dto.TargetResource,
                ParametersJson = dto.ParametersJson,
                PreviewText = dto.PreviewText,
                RiskLevel = dto.RiskLevel,
                Status = dto.Status,
                CreatedAtUtc = dto.CreatedAtUtc,
                ConfirmedAtUtc = dto.ConfirmedAtUtc,
                ExecutedAtUtc = dto.ExecutedAtUtc,
                ExecutionResultJson = dto.ExecutionResultJson
            };
        }
    }
}