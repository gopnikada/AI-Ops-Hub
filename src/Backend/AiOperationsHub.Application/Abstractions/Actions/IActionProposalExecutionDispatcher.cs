namespace AiOperationsHub.Application.Abstractions.Actions
{
    using AiOperationsHub.Application.Actions.Execution;
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Resolves the correct executor for an action proposal and executes it.
    /// </summary>
    public interface IActionProposalExecutionDispatcher
    {
        /// <summary>
        /// Executes the specified action proposal using a matching executor.
        /// </summary>
        /// <param name="proposal">The proposal to execute.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>The execution result.</returns>
        Task<ActionProposalExecutionResult> ExecuteAsync(
            ActionProposal proposal,
            CancellationToken cancellationToken);
    }
}