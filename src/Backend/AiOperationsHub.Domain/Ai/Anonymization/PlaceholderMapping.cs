namespace AiOperationsHub.Domain.Ai.Anonymization
{
    public sealed class PlaceholderMapping
    {
        public Guid Id { get; set; }
        public SensitiveEntityType EntityType { get; set; }
        public string OriginalValue { get; set; } = null!;
        public string Placeholder { get; set; } = null!;
    }
}
