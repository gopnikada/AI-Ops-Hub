namespace AiOperationsHub.Application.Abstractions.Providers
{
    /// <summary>
    /// Defines a provider-agnostic AI generation contract for application use cases.
    /// </summary>
    public interface IAiProvider
    {
        /// <summary>
        /// Generates provider output for the supplied request.
        /// </summary>
        /// <param name="request">The normalized AI provider request.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the normalized provider response.</returns>
        Task<AiProviderResponse> GenerateAsync(
            AiProviderRequest request,
            CancellationToken cancellationToken);
    }
}