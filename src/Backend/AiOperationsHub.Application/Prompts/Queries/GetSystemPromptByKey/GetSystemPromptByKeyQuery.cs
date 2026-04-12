namespace AiOperationsHub.Application.Prompts.Queries.GetSystemPromptByKey
{
    using AiOperationsHub.Application.Prompts.Dtos;
    using MediatR;

    /// <summary>
    /// Gets a system prompt by key.
    /// </summary>
    public sealed class GetSystemPromptByKeyQuery : IRequest<SystemPromptDto?>
    {
        /// <summary>
        /// Gets or sets the prompt key.
        /// </summary>
        public string Key { get; set; } = null!;
    }
}