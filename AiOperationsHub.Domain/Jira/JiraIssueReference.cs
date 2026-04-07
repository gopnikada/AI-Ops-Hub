namespace AiOperationsHub.Domain.Jira
{
    public sealed class JiraIssueReference
    {
        public string IssueKey { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string IssueType { get; set; } = null!;
    }
}
