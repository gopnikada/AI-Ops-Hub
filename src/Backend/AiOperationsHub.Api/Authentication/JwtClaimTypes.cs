namespace AiOperationsHub.Api.Authentication
{
    /// <summary>
    /// Defines JWT claim type names used by the API.
    /// </summary>
    public static class JwtClaimTypes
    {
        /// <summary>
        /// The subject claim type.
        /// </summary>
        public const string Subject = "sub";

        /// <summary>
        /// The role claim type.
        /// </summary>
        public const string Role = "role";
    }
}