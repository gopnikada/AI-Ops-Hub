namespace AiOperationsHub.Domain.Audit
{
    public enum AuditEventType
    {
        ConversationStarted = 1,
        UserPromptReceived = 2,
        IntentClassified = 3,
        ConnectorSelected = 4,
        SourceAccessRequested = 5,
        SourceAccessGranted = 6,
        SourceAccessDenied = 7,
        ContentRetrieved = 8,
        ContentMinimized = 9,
        ContentAnonymized = 10,
        ProviderRequestPrepared = 11,
        ProviderCalled = 12,
        ProviderResponded = 13,
        ProviderCallBlocked = 14,
        ActionProposed = 15,
        ActionValidationCompleted = 16,
        ActionPreviewShown = 17,
        ActionConfirmationReceived = 18,
        ActionExecutionStarted = 19,
        ActionExecutionSucceeded = 20,
        ActionExecutionFailed = 21
    }
}
