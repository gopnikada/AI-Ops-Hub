namespace AiOperationsHub.Application.Abstractions.Persistence
{
    /// <summary>
    /// Represents a unit-of-work boundary for persisting application changes atomically.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Persists all tracked changes to the underlying data store.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the changes have been saved.</returns>
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}