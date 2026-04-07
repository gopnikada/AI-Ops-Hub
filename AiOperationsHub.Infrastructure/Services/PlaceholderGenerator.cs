using System.Security.Cryptography;
using System.Text;

namespace AiOperationsHub.Infrastructure.Services
{
    /// <summary>
    /// Generates deterministic placeholders for anonymized values.
    /// </summary>
    public sealed class PlaceholderGenerator
    {
        /// <summary>
        /// Generates a placeholder string for the specified category and source value.
        /// </summary>
        /// <param name="category">The placeholder category.</param>
        /// <param name="source">The original source value.</param>
        /// <returns>A deterministic placeholder.</returns>
        public string Generate(string category, string source)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                throw new ArgumentException("Category is required.", nameof(category));
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                return $"[{category.ToUpperInvariant()}_REDACTED]";
            }

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(source.Trim()));
            var token = Convert.ToHexString(hash)[..8];

            return $"[{category.ToUpperInvariant()}_{token}]";
        }
    }
}