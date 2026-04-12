namespace AiOperationsHub.Application.Prompts.Commands.UpsertSystemPrompt
{
    using AiOperationsHub.Application.Abstractions.Persistence;
    using AiOperationsHub.Application.Abstractions.Repositories;
    using AiOperationsHub.Application.Prompts.Dtos;
    using AiOperationsHub.Domain.Configuration;
    using MediatR;

    /// <summary>
    /// Handles <see cref="UpsertSystemPromptCommand"/>.
    /// </summary>
    public sealed class UpsertSystemPromptCommandHandler
        : IRequestHandler<UpsertSystemPromptCommand, SystemPromptDto>
    {
        private readonly ISystemPromptRepository _systemPromptRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpsertSystemPromptCommandHandler"/> class.
        /// </summary>
        /// <param name="systemPromptRepository">The system prompt repository.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public UpsertSystemPromptCommandHandler(
            ISystemPromptRepository systemPromptRepository,
            IUnitOfWork unitOfWork)
        {
            _systemPromptRepository = systemPromptRepository;
            _unitOfWork = unitOfWork;
        }

        /// <inheritdoc />
        public async Task<SystemPromptDto> Handle(
            UpsertSystemPromptCommand request,
            CancellationToken cancellationToken)
        {
            var saved = await _systemPromptRepository.UpsertAsync(
                new SystemPromptSetting
                {
                    Key = request.Key,
                    Value = request.Value,
                    UpdatedByUserId = request.UpdatedByUserId,
                    UpdatedUtc = DateTime.UtcNow
                },
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SystemPromptDto
            {
                Key = saved.Key,
                Value = saved.Value,
                UpdatedByUserId = saved.UpdatedByUserId,
                UpdatedUtc = saved.UpdatedUtc
            };
        }
    }
}