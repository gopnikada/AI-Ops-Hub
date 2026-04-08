using AiOperationsHub.Application.Abstractions.Persistence;
using AiOperationsHub.Persistence.Db;

namespace AiOperationsHub.Persistence.Repositories
{
    /// <summary>
    /// Provides an EF Core-backed unit-of-work implementation.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="dbContext">The application database context.</param>
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Persists all tracked changes to the database.
        /// </summary>
        /// <param name="cancellationToken">A token used to cancel the asynchronous operation.</param>
        /// <returns>A task that completes when the changes have been saved.</returns>
        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}