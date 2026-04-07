namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents the authenticated user context available to application use cases.
    /// </summary>
    public sealed class CurrentUserContext
    {
        /// <summary>
        /// Gets or sets the unique identifier of the current user.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name or login name of the current user.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the roles assigned to the current user.
        /// </summary>
        public IReadOnlyCollection<string> Roles { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Determines whether the current user belongs to the specified role.
        /// </summary>
        /// <param name="role">The role name to evaluate.</param>
        /// <returns><c>true</c> when the user is in the role; otherwise <c>false</c>.</returns>
        public bool IsInRole(string role)
        {
            return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}