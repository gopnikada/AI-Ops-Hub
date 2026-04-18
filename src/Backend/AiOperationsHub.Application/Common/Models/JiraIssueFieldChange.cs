namespace AiOperationsHub.Application.Common.Models
{
    /// <summary>
    /// Represents one planned Jira issue field change.
    /// </summary>
    public sealed class JiraIssueFieldChange
    {
        /// <summary>
        /// Gets or sets the logical field name.
        /// </summary>
        public string FieldName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current value.
        /// </summary>
        public string? CurrentValue { get; set; }

        /// <summary>
        /// Gets or sets the proposed value.
        /// </summary>
        public string? ProposedValue { get; set; }
    }
}