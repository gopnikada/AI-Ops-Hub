namespace AiOperationsHub.Application.Tools.Planning
{
    /// <summary>
    /// Selects an internal tool and arguments from a plain-language user request.
    /// </summary>
    public interface IAiToolPlanner
    {
        /// <summary>
        /// Plans the next tool invocation.
        /// </summary>
        /// <param name="request">The planning request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The planning response.</returns>
        Task<ToolPlanningResponse> PlanAsync(
            ToolPlanningRequest request,
            CancellationToken cancellationToken);
    }
}