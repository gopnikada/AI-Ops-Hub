using AiOperationsHub.Api.Authentication;
using System.Security.Claims;

namespace AiOperationsHub.Api.Extensions
{
    /// <summary>
    /// Provides helper methods for reading typed values from the current principal.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the current user identifier from the principal.
        /// </summary>
        /// <param name="principal">The current authenticated principal.</param>
        /// <returns>The parsed user identifier.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the subject claim is missing or invalid.
        /// </exception>
        public static Guid GetRequiredUserId(this ClaimsPrincipal principal)
        {
            var subject = principal.FindFirstValue(JwtClaimTypes.Subject)
                ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(subject, out var userId))
            {
                throw new InvalidOperationException("The authenticated user does not contain a valid subject identifier.");
            }

            return userId;
        }
    }
}