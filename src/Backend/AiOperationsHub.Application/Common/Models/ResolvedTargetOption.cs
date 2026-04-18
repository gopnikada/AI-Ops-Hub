namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents one concrete candidate returned by target-resource resolution.
    /// </summary>
    public sealed class ResolvedTargetOption
    {
        /// <summary>
        /// Gets or sets the concrete identifier that should later be used by the action flow.
        /// </summary>
        public string Identifier { get; set; } = null!;

        /// <summary>
        /// Gets or sets the primary display text for the candidate.
        /// </summary>
        public string DisplayName { get; set; } = null!;

        /// <summary>
        /// Gets or sets optional secondary text that helps disambiguate the candidate.
        /// </summary>
        public string? SecondaryText { get; set; }

        /// <summary>
        /// Gets or sets the optional external URL for the candidate.
        /// </summary>
        public string? Url { get; set; }
    }
}