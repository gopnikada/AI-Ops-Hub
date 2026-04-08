namespace AiOperationsHub.Domain.Ai.Anonymization
{
    public enum SensitiveEntityType
    {
        Person = 1,
        Email = 2,
        Phone = 3,
        Address = 4,
        AccountNumber = 5,
        FreeTextPii = 6,
        Other = 99
    }
}
