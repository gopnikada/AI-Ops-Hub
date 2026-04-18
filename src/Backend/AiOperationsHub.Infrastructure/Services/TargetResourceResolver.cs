namespace AiOperationsHub.Infrastructure.Services
{
    using AiOperationsHub.Application.Abstractions.Jira;
    using AiOperationsHub.Application.Abstractions.Resolution;
    using AiOperationsHub.Application.Common.Models;
    using AiOperationsHub.Domain.Actions;

    /// <summary>
    /// Resolves target resources across supported external systems.
    /// </summary>
    public sealed class TargetResourceResolver : ITargetResourceResolver
    {
        private readonly IJiraConnector _jiraConnector;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetResourceResolver"/> class.
        /// </summary>
        /// <param name="jiraConnector">The Jira connector.</param>
        public TargetResourceResolver(IJiraConnector jiraConnector)
        {
            _jiraConnector = jiraConnector;
        }

        /// <inheritdoc />
        public async Task<ResolveTargetResourceResult> ResolveAsync(
            ResolveTargetResourceRequest request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.TargetSystem != ActionTargetSystem.Jira)
            {
                throw new InvalidOperationException(
                    $"Target resource resolution is not implemented for target system '{request.TargetSystem}'.");
            }

            var jiraMatches = await _jiraConnector.SearchIssuesAsync(
                new ResolveJiraIssueRequest
                {
                    ProjectKey = request.ScopeKey,
                    IssueReference = request.Reference
                },
                cancellationToken);

            var options = jiraMatches
                .Select(match => new ResolvedTargetOption
                {
                    Identifier = match.IssueKey,
                    DisplayName = $"{match.IssueKey} - {match.Summary}",
                    SecondaryText = match.Description,
                    Url = match.IssueUrl
                })
                .ToArray();

            var status = options.Length switch
            {
                0 => TargetResourceResolutionStatus.NoMatches,
                1 => TargetResourceResolutionStatus.SingleMatch,
                _ => TargetResourceResolutionStatus.MultipleMatches
            };

            return new ResolveTargetResourceResult
            {
                TargetSystem = request.TargetSystem,
                ScopeKey = request.ScopeKey,
                Reference = request.Reference,
                Status = status,
                ResolvedIdentifier = options.Length == 1 ? options[0].Identifier : null,
                Matches = options
            };
        }
    }
}