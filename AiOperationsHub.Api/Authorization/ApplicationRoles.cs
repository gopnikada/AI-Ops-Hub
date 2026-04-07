namespace AiOperationsHub.Api.Authorization
{
    /// <summary>
    /// Defines application role names used by the API.
    /// </summary>
    public static class ApplicationRoles
    {
        /// <summary>
        /// Administrative role.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Operational role.
        /// </summary>
        public const string Operator = "Operator";

        /// <summary>
        /// Audit/read-only role.
        /// </summary>
        public const string Auditor = "Auditor";
    }
}