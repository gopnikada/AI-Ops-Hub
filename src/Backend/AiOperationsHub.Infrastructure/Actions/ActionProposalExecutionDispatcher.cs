namespace AiOperationsHub.Infrastructure.Actions
{
    using AiOperationsHub.Application.Abstractions.Actions;
    using AiOperationsHub.Application.Actions.Execution;
    using AiOperationsHub.Domain.Actions;
    using AiOperationsHub.Domain.Common;

    /// <summary>
    /// Default action proposal execution dispatcher.
    /// </summary>
    public sealed class ActionProposalExecutionDispatcher : IActionProposalExecutionDispatcher
    {
        private readonly IReadOnlyCollection<IActionProposalExecutor> _executors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionProposalExecutionDispatcher"/> class.
        /// </summary>
        /// <param name="executors">The registered executors.</param>
        public ActionProposalExecutionDispatcher(IEnumerable<IActionProposalExecutor> executors)
        {
            _executors = executors.ToArray();
        }

        /// <inheritdoc />
        public async Task<ActionProposalExecutionResult> ExecuteAsync(
            ActionProposal proposal,
            CancellationToken cancellationToken)
        {
            var matches = _executors
                .Where(x => x.CanExecute(proposal))
                .ToArray();

            if (matches.Length == 0)
            {
                throw new DomainException(
                    $"No executor is registered for target system '{proposal.TargetSystem}' and action '{proposal.ActionName}'.");
            }

            if (matches.Length > 1)
            {
                throw new DomainException(
                    $"Multiple executors are registered for target system '{proposal.TargetSystem}' and action '{proposal.ActionName}'.");
            }

            var executor = matches[0];

            return await executor.ExecuteAsync(proposal, cancellationToken);
        }
    }
}