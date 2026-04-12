namespace AiOperationsHub.Persistence.Repositories
{
    using AiOperationsHub.Application.Abstractions.Repositories;
    using AiOperationsHub.Domain.Configuration;
    using AiOperationsHub.Persistence.Db;
    using AiOperationsHub.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Persists system prompt settings in SQL Server.
    /// </summary>
    public sealed class SystemPromptRepository : ISystemPromptRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemPromptRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public SystemPromptRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<SystemPromptSetting?> GetByKeyAsync(
            string key,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.SystemPromptSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Key == key, cancellationToken);

            if (entity is null)
            {
                return null;
            }

            return new SystemPromptSetting
            {
                Key = entity.Key,
                Value = entity.Value,
                UpdatedByUserId = entity.UpdatedByUserId,
                UpdatedUtc = entity.UpdatedUtc
            };
        }

        /// <inheritdoc />
        public async Task<SystemPromptSetting> UpsertAsync(
            SystemPromptSetting setting,
            CancellationToken cancellationToken)
        {
            var entity = await _dbContext.SystemPromptSettings
                .FirstOrDefaultAsync(x => x.Key == setting.Key, cancellationToken);

            if (entity is null)
            {
                entity = new SystemPromptSettingDbEntity
                {
                    Key = setting.Key,
                    Value = setting.Value,
                    UpdatedByUserId = setting.UpdatedByUserId,
                    UpdatedUtc = setting.UpdatedUtc
                };

                await _dbContext.SystemPromptSettings.AddAsync(entity, cancellationToken);
            }
            else
            {
                entity.Value = setting.Value;
                entity.UpdatedByUserId = setting.UpdatedByUserId;
                entity.UpdatedUtc = setting.UpdatedUtc;
            }

            return new SystemPromptSetting
            {
                Key = entity.Key,
                Value = entity.Value,
                UpdatedByUserId = entity.UpdatedByUserId,
                UpdatedUtc = entity.UpdatedUtc
            };
        }
    }
}