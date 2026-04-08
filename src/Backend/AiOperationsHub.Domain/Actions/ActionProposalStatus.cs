namespace AiOperationsHub.Domain.Actions
{
    public enum ActionProposalStatus
    {
        Drafted = 1,
        AwaitingConfirmation = 2,
        Confirmed = 3,
        Executing = 4,
        Executed = 5,
        Failed = 6,
        Rejected = 7,
        Expired = 8
    }
}
