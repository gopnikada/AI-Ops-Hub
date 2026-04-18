namespace AiOperationsHub.Api.Contracts.Responses
{
    using AiOperationsHub.Application.Actions.Dtos;

    /// <summary>
    /// Represents the HTTP response for proposal preparation, which may return a proposal or a target-resolution result.
    /// </summary>
    public sealed class ProposalPreparationResponse
    {
        /// <summary>
        /// Gets or sets the prepared proposal when available.
        /// </summary>
        public ActionProposalResponse? Proposal { get; set; }

        /// <summary>
        /// Gets or sets the target-resolution result when target selection is required.
        /// </summary>
        public TargetResourceResolutionResponse? Resolution { get; set; }

        /// <summary>
        /// Maps an application-layer result to an API response.
        /// </summary>
        /// <param name="source">The source result.</param>
        /// <returns>The mapped response.</returns>
        public static ProposalPreparationResponse FromDto(ProposalPreparationResultDto source)
        {
            return new ProposalPreparationResponse
            {
                Proposal = source.Proposal is null
                    ? null
                    : ActionProposalResponse.FromDto(source.Proposal),
                Resolution = source.Resolution is null
                    ? null
                    : TargetResourceResolutionResponse.FromModel(source.Resolution)
            };
        }
    }
}