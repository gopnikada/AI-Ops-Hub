using AiOperationsHub.Persistence.Configurations;
using AiOperationsHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AiOperationsHub.Persistence.Db
{
    /// <summary>
    /// Represents the primary EF Core database context for AI Operations Hub.
    /// </summary>
    public sealed class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The EF Core options used to configure the context.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets the database set for persisted action proposals.
        /// </summary>
        public DbSet<ActionProposalDbEntity> ActionProposals => Set<ActionProposalDbEntity>();

        /// <summary>
        /// Gets the database set for persisted audit events.
        /// </summary>
        public DbSet<AuditEventDbEntity> AuditEvents => Set<AuditEventDbEntity>();

        public DbSet<SystemPromptSettingDbEntity> SystemPromptSettings => Set<SystemPromptSettingDbEntity>();

        /// <summary>
        /// Applies entity configurations for the persistence model.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure EF Core entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            modelBuilder.ApplyConfiguration(new SystemPromptSettingConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}