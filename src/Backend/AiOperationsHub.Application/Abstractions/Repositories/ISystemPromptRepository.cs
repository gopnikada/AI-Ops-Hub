namespace AiOperationsHub.Application.Abstractions.Repositories
{
    using AiOperationsHub.Domain.Configuration;

    /// <summary>
    /// Provides persistence operations for system prompt settings.
    /// </summary>
    public interface ISystemPromptRepository
    {
        /// <summary>
        /// Gets a system prompt by its unique key.
        /// </summary>
        /// <param name="key">The prompt key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The prompt setting, or null when not found.</returns>
        Task<SystemPromptSetting?> GetByKeyAsync(
            string key,
            CancellationToken cancellationToken);

        /// <summary>
        /// Creates or updates a system prompt setting.
        /// </summary>
        /// <param name="setting">The setting to persist.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The persisted setting.</returns>
        Task<SystemPromptSetting> UpsertAsync(
            SystemPromptSetting setting,
            CancellationToken cancellationToken);
    }
}