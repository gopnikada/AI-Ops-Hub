using System.ComponentModel.DataAnnotations;

namespace AiOperationsHub.Api.Authentication
{
    /// <summary>
    /// Represents JWT authentication settings bound from configuration.
    /// </summary>
    public sealed class JwtOptions
    {
        /// <summary>
        /// The configuration section name.
        /// </summary>
        public const string SectionName = "Jwt";

        /// <summary>
        /// Gets or sets the token issuer.
        /// </summary>
        [Required]
        public string Issuer { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token audience.
        /// </summary>
        [Required]
        public string Audience { get; set; } = null!;

        /// <summary>
        /// Gets or sets the symmetric signing key.
        /// </summary>
        [Required]
        [MinLength(32)]
        public string SigningKey { get; set; } = null!;
    }
}