namespace AiOperationsHub.Application.Prompts.Commands.UpsertSystemPrompt
{
    using AiOperationsHub.Application.Prompts.Dtos;
    using MediatR;

    /// <summary>
    /// Creates or updates a system prompt setting.
    /// </summary>
    public sealed class UpsertSystemPromptCommand : IRequest<SystemPromptDto>
    {
        /// <summary>
        /// Gets or sets the prompt key.
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// Gets or sets the prompt value.
        /// </summary>
        public string Value { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user identifier performing the update.
        /// </summary>
        public Guid UpdatedByUserId { get; set; }
    }
}