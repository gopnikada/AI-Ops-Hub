namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Describes the outcome of resolving a target resource reference.
    /// </summary>
    public enum TargetResourceResolutionStatus
    {
        /// <summary>
        /// No matching resources were found.
        /// </summary>
        NoMatches = 1,

        /// <summary>
        /// Exactly one matching resource was found.
        /// </summary>
        SingleMatch = 2,

        /// <summary>
        /// Multiple matching resources were found and user selection is required.
        /// </summary>
        MultipleMatches = 3
    }
}