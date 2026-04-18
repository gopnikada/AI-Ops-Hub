using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Domain.Actions;
using AiOperationsHub.Domain.Common;
using AiOperationsHub.Persistence.Db;
using AiOperationsHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AiOperationsHub.Persistence.Repositories
{
    /// <summary>
    /// Provides EF Core-backed persistence operations for action proposals.
    /// </summary>
    public sealed class ActionProposalRepository : IActionProposalRepository
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionProposalRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context.</param>
        public ActionProposalRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a new action proposal to the database context.
        /// </summary>
        /// <param name="proposal">The proposal to add.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the proposal has been added to the change tracker.</returns>
        public Task AddAsync(ActionProposal proposal, CancellationToken cancellationToken)
        {
            var entity = new ActionProposalDbEntity
            {
                Id = proposal.Id,
                RequestedByUserId = proposal.RequestedByUserId,
                TargetSystem = (int)proposal.TargetSystem,
                ActionName = proposal.ActionName,
                TargetResource = proposal.TargetResource,
                ParametersJson = proposal.ParametersJson,
                PreviewText = proposal.PreviewText,
                RiskLevel = (int)proposal.RiskLevel,
                Status = (int)proposal.Status,
                CreatedAtUtc = proposal.CreatedAtUtc,
                UpdatedAtUtc = proposal.UpdatedAtUtc,
                ConfirmedAtUtc = proposal.ConfirmedAtUtc,
                ExecutedAtUtc = proposal.ExecutedAtUtc,
                ExecutionResultJson = proposal.ExecutionResultJson
            };

            _dbContext.ActionProposals.Add(entity);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves an action proposal by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the proposal to retrieve.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task containing the proposal when found; otherwise <c>null</c>.</returns>
        public async Task<ActionProposal?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.ActionProposals
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity is null)
            {
                return null;
            }

            return ActionProposal.Rehydrate(
                entity.Id,
                entity.RequestedByUserId,
                (ActionTargetSystem)entity.TargetSystem,
                entity.ActionName,
                entity.TargetResource,
                entity.ParametersJson,
                entity.PreviewText,
                (ActionRiskLevel)entity.RiskLevel,
                (ActionProposalStatus)entity.Status,
                entity.CreatedAtUtc,
                entity.UpdatedAtUtc,
                entity.ConfirmedAtUtc,
                entity.ExecutedAtUtc,
                entity.ExecutionResultJson);
        }

        /// <summary>
        /// Updates an existing action proposal in the database context.
        /// </summary>
        /// <param name="proposal">The updated proposal state.</param>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the proposal has been staged for update.</returns>
        public async Task UpdateAsync(ActionProposal proposal, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.ActionProposals
                .FirstOrDefaultAsync(x => x.Id == proposal.Id, cancellationToken);

            if (entity is null)
            {
                throw new DomainException($"Action proposal '{proposal.Id}' was not found.");
            }

            entity.RequestedByUserId = proposal.RequestedByUserId;
            entity.TargetSystem = (int)proposal.TargetSystem;
            entity.ActionName = proposal.ActionName;
            entity.TargetResource = proposal.TargetResource;
            entity.ParametersJson = proposal.ParametersJson;
            entity.PreviewText = proposal.PreviewText;
            entity.RiskLevel = (int)proposal.RiskLevel;
            entity.Status = (int)proposal.Status;
            entity.CreatedAtUtc = proposal.CreatedAtUtc;
            entity.UpdatedAtUtc = proposal.UpdatedAtUtc;
            entity.ConfirmedAtUtc = proposal.ConfirmedAtUtc;
            entity.ExecutedAtUtc = proposal.ExecutedAtUtc;
            entity.ExecutionResultJson = proposal.ExecutionResultJson;
        }
    }
}