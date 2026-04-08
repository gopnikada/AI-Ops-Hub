using AiOperationsHub.Domain.Ai.Anonymization;

namespace AiOperationsHub.Application.Abstractions.Security
{
    /// <summary>
    /// Detects and anonymizes sensitive content before external provider transmission.
    /// </summary>
    public interface IAnonymizationService
    {
        /// <summary>
        /// Produces a sanitized representation of the supplied input while preserving internal mappings.
        /// </summary>
        /// <param name="input">The raw text to inspect and anonymize.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the anonymized text result and placeholder mappings.</returns>
        Task<AnonymizedText> AnonymizeAsync(
            string input,
            CancellationToken cancellationToken);
    }
}