namespace AiOperationsHub.Api.Contracts.Responses
{
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Represents the HTTP response for generic target-resource resolution.
    /// </summary>
    public sealed class TargetResourceResolutionResponse
    {
        /// <summary>
        /// Gets or sets the target system that was resolved.
        /// </summary>
        public ActionTargetSystem TargetSystem { get; set; }

        /// <summary>
        /// Gets or sets the optional scope key used during resolution.
        /// </summary>
        public string? ScopeKey { get; set; }

        /// <summary>
        /// Gets or sets the original reference text.
        /// </summary>
        public string Reference { get; set; } = null!;

        /// <summary>
        /// Gets or sets the resolution status.
        /// </summary>
        public TargetResourceResolutionStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the resolved identifier when exactly one match was found.
        /// </summary>
        public string? ResolvedIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the returned resolution candidates.
        /// </summary>
        public IReadOnlyCollection<TargetResourceResolutionOptionResponse> Matches { get; set; } =
            Array.Empty<TargetResourceResolutionOptionResponse>();

        /// <summary>
        /// Maps an application-layer resolution result to an API response.
        /// </summary>
        /// <param name="source">The source result.</param>
        /// <returns>The mapped response.</returns>
        public static TargetResourceResolutionResponse FromModel(ResolveTargetResourceResult source)
        {
            return new TargetResourceResolutionResponse
            {
                TargetSystem = source.TargetSystem,
                ScopeKey = source.ScopeKey,
                Reference = source.Reference,
                Status = source.Status,
                ResolvedIdentifier = source.ResolvedIdentifier,
                Matches = source.Matches
                    .Select(x => new TargetResourceResolutionOptionResponse
                    {
                        Identifier = x.Identifier,
                        DisplayName = x.DisplayName,
                        SecondaryText = x.SecondaryText,
                        Url = x.Url
                    })
                    .ToArray()
            };
        }
    }

    /// <summary>
    /// Represents one target-resource candidate in an API response.
    /// </summary>
    public sealed class TargetResourceResolutionOptionResponse
    {
        /// <summary>
        /// Gets or sets the candidate identifier.
        /// </summary>
        public string Identifier { get; set; } = null!;

        /// <summary>
        /// Gets or sets the primary display text.
        /// </summary>
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the optional secondary text.
        /// </summary>
        public string? SecondaryText { get; set; }

        /// <summary>
        /// Gets or sets the optional external URL.
        /// </summary>
        public string? Url { get; set; }
    }
}