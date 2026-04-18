namespace AiOperationsHub.Domain.Actions
{
    /// <summary>
    /// Defines supported Jira action types.
    /// </summary>
    public enum JiraActionType
    {
        /// <summary>
        /// Creates a Jira issue.
        /// </summary>
        CreateIssue = 1,

        /// <summary>
        /// Edits an existing Jira issue.
        /// </summary>
        EditIssue = 2
    }
}