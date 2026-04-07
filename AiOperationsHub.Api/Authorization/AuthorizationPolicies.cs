namespace AiOperationsHub.Api.Authorization
{
    /// <summary>
    /// Defines authorization policy names.
    /// </summary>
    public static class AuthorizationPolicies
    {
        /// <summary>
        /// Policy for reading action proposals.
        /// </summary>
        public const string CanReadProposals = "CanReadProposals";

        /// <summary>
        /// Policy for creating action proposals.
        /// </summary>
        public const string CanCreateProposals = "CanCreateProposals";

        /// <summary>
        /// Policy for confirming action proposals.
        /// </summary>
        public const string CanConfirmProposals = "CanConfirmProposals";
    }
}