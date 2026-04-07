namespace AiOperationsHub.Domain.Ai.Anonymization
{
    public sealed class AnonymizedText
    {
        public string OriginalText { get; set; } = null!;
        public string SanitizedText { get; set; } = null!;
        public bool ContainsSensitiveData { get; set; }
        public List<PlaceholderMapping> Mappings { get; set; } = new();
    }
}
