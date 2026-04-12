namespace AiOperationsHub.Application.Prompts.Queries.GetSystemPromptByKey
{
    using AiOperationsHub.Application.Abstractions.Repositories;
    using AiOperationsHub.Application.Prompts.Dtos;
    using MediatR;

    /// <summary>
    /// Handles <see cref="GetSystemPromptByKeyQuery"/>.
    /// </summary>
    public sealed class GetSystemPromptByKeyQueryHandler
        : IRequestHandler<GetSystemPromptByKeyQuery, SystemPromptDto?>
    {
        private readonly ISystemPromptRepository _systemPromptRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSystemPromptByKeyQueryHandler"/> class.
        /// </summary>
        /// <param name="systemPromptRepository">The system prompt repository.</param>
        public GetSystemPromptByKeyQueryHandler(
            ISystemPromptRepository systemPromptRepository)
        {
            _systemPromptRepository = systemPromptRepository;
        }

        /// <inheritdoc />
        public async Task<SystemPromptDto?> Handle(
            GetSystemPromptByKeyQuery request,
            CancellationToken cancellationToken)
        {
            var setting = await _systemPromptRepository.GetByKeyAsync(
                request.Key,
                cancellationToken);

            if (setting is null)
            {
                return null;
            }

            return new SystemPromptDto
            {
                Key = setting.Key,
                Value = setting.Value,
                UpdatedByUserId = setting.UpdatedByUserId,
                UpdatedUtc = setting.UpdatedUtc
            };
        }
    }
}