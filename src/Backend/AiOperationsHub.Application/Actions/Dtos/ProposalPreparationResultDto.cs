namespace AiOperationsHub.Application.Actions.Dtos
{
    using AiOperationsHub.Application.Common.Models;

    /// <summary>
    /// Represents the result of preparing an action proposal, which may either yield a proposal or require target selection.
    /// </summary>
    public sealed class ProposalPreparationResultDto
    {
        /// <summary>
        /// Gets or sets the created proposal when proposal preparation completed successfully.
        /// </summary>
        public ActionProposalDto? Proposal { get; set; }

        /// <summary>
        /// Gets or sets the target-resolution result when a concrete target still needs to be selected.
        /// </summary>
        public ResolveTargetResourceResult? Resolution { get; set; }
    }
}