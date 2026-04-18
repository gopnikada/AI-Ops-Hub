namespace AiOperationsHub.Application.Abstractions.Actions
{
    using AiOperationsHub.Application.Actions.Execution;
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Executes a previously confirmed action proposal.
    /// </summary>
    public interface IActionProposalExecutor
    {
        /// <summary>
        /// Determines whether the executor can handle the specified proposal.
        /// </summary>
        /// <param name="proposal">The proposal to evaluate.</param>
        /// <returns><see langword="true"/> when the executor can handle the proposal; otherwise, <see langword="false"/>.</returns>
        bool CanExecute(ActionProposal proposal);

        /// <summary>
        /// Executes the specified proposal.
        /// </summary>
        /// <param name="proposal">The proposal to execute.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>The execution result.</returns>
        Task<ActionProposalExecutionResult> ExecuteAsync(
            ActionProposal proposal,
            CancellationToken cancellationToken);
    }
}