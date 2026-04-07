using System.Text.RegularExpressions;
using AiOperationsHub.Application.Abstractions.Security;
using AiOperationsHub.Domain.Ai.Anonymization;
using Microsoft.Extensions.Logging;

namespace AiOperationsHub.Infrastructure.Services
{
    /// <summary>
    /// Provides a basic structured anonymization implementation for outbound provider requests.
    /// </summary>
    public sealed class StructuredAnonymizationService : IAnonymizationService
    {
        private static readonly Regex EmailRegex =
            new(@"(?<![\w\.\-])([A-Z0-9._%+\-]+@[A-Z0-9.\-]+\.[A-Z]{2,})(?![\w\.\-])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex GuidRegex =
            new(@"\b[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}\b", RegexOptions.Compiled);

        private static readonly Regex UrlRegex =
            new(@"\bhttps?:\/\/[^\s]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly PlaceholderGenerator _placeholderGenerator;
        private readonly ILogger<StructuredAnonymizationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredAnonymizationService"/> class.
        /// </summary>
        /// <param name="placeholderGenerator">The placeholder generator.</param>
        /// <param name="logger">The logger.</param>
        public StructuredAnonymizationService(
            PlaceholderGenerator placeholderGenerator,
            ILogger<StructuredAnonymizationService> logger)
        {
            _placeholderGenerator = placeholderGenerator;
            _logger = logger;
        }

        /// <summary>
        /// Produces a sanitized representation of the supplied input while preserving internal mappings.
        /// </summary>
        /// <param name="input">The raw text to inspect and anonymize.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the anonymized text result and placeholder mappings.</returns>
        public Task<AnonymizedText> AnonymizeAsync(
            string input,
            CancellationToken cancellationToken)
        {
            input ??= string.Empty;

            var sanitized = input;
            var containsSensitiveData = false;

            sanitized = EmailRegex.Replace(sanitized, match =>
            {
                containsSensitiveData = true;
                return _placeholderGenerator.Generate("email", match.Value);
            });

            sanitized = GuidRegex.Replace(sanitized, match =>
            {
                containsSensitiveData = true;
                return _placeholderGenerator.Generate("guid", match.Value);
            });

            sanitized = UrlRegex.Replace(sanitized, match =>
            {
                containsSensitiveData = true;
                return _placeholderGenerator.Generate("url", match.Value);
            });

            var result = new AnonymizedText
            {
                OriginalText = input,
                SanitizedText = sanitized,
                ContainsSensitiveData = containsSensitiveData,
                Mappings = new()
            };

            _logger.LogDebug(
                "Anonymization completed. ContainsSensitiveData: {ContainsSensitiveData}",
                containsSensitiveData);

            return Task.FromResult(result);
        }
    }
}