namespace AiOperationsHub.Application.Actions.Commands.CreateJiraIssueEditProposal
{
    using System.Text;
    using AiOperationsHub.Application.Common.Models;

    /// <summary>
    /// Builds preview text for Jira issue edit proposals.
    /// </summary>
    internal static class JiraIssueEditPreviewBuilder
    {
        /// <summary>
        /// Builds proposal preview markdown for a Jira issue edit.
        /// </summary>
        /// <param name="issueKey">The resolved Jira issue key.</param>
        /// <param name="currentSummary">The current issue summary.</param>
        /// <param name="changes">The planned change set.</param>
        /// <returns>The preview markdown.</returns>
        public static string Build(
            string issueKey,
            string currentSummary,
            IReadOnlyCollection<JiraIssueFieldChange> changes)
        {
            var builder = new StringBuilder();

            builder.AppendLine("### Jira Issue Edit Draft");
            builder.AppendLine();
            builder.AppendLine($"**Issue:** `{issueKey}`  ");
            builder.AppendLine($"**Current Summary:** `{currentSummary}`  ");
            builder.AppendLine();
            builder.AppendLine("---");
            builder.AppendLine();
            builder.AppendLine("### Planned Changes");
            builder.AppendLine();

            foreach (var change in changes)
            {
                builder.AppendLine($"- **{change.FieldName}**");
                builder.AppendLine($"  - Current: `{change.CurrentValue ?? "[None]"}`");
                builder.AppendLine($"  - Proposed: `{change.ProposedValue ?? "[None]"}`");
            }

            return builder.ToString().TrimEnd();
        }
    }
}