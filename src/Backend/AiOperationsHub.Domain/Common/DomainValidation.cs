namespace AiOperationsHub.Domain.Common
{
    public static class DomainValidation
    {
        public static void RequireNotNullOrWhiteSpace(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{fieldName} is required.");
        }

        public static void RequireMaxLength(string? value, int maxLength, string fieldName)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
                throw new DomainException($"{fieldName} must not exceed {maxLength} characters.");
        }

        public static void RequireTrue(bool condition, string message)
        {
            if (!condition)
                throw new DomainException(message);
        }
    }
}
