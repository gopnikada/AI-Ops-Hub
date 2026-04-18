using AiOperationsHub.Domain.Actions;

namespace AiOperationsHub.Application.Abstractions.Persistence
{
    /// <summary>
    /// Provides persistence operations for action proposals.
    /// </summary>
    public interface IActionProposalRepository
    {
        /// <summary>
        /// Adds a new action proposal to the persistence store.
        /// </summary>
        /// <param name="proposal">The action proposal to add.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the proposal has been staged for persistence.</returns>
        Task AddAsync(ActionProposal proposal, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an action proposal by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the proposal to retrieve.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>
        /// A task containing the matching <see cref="ActionProposal"/> when found; otherwise <c>null</c>.
        /// </returns>
        Task<ActionProposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing action proposal in the persistence store.
        /// </summary>
        /// <param name="proposal">The updated action proposal state.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the proposal has been staged for update.</returns>
        Task UpdateAsync(ActionProposal proposal, CancellationToken cancellationToken);
    }
}