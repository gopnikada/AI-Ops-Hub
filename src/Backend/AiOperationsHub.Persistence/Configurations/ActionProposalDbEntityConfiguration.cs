using AiOperationsHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiOperationsHub.Persistence.Configurations
{
    /// <summary>
    /// Configures the EF Core mapping for <see cref="ActionProposalDbEntity"/>.
    /// </summary>
    public sealed class ActionProposalDbEntityConfiguration : IEntityTypeConfiguration<ActionProposalDbEntity>
    {
        /// <summary>
        /// Configures the database mapping for the action proposal entity.
        /// </summary>
        /// <param name="builder">The entity type builder.</param>
        public void Configure(EntityTypeBuilder<ActionProposalDbEntity> builder)
        {
            builder.ToTable("ActionProposals");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RequestedByUserId)
                .IsRequired();

            builder.Property(x => x.TargetSystem)
                .IsRequired();

            builder.Property(x => x.ActionName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.TargetResource)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ParametersJson)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.PreviewText)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

            builder.Property(x => x.RiskLevel)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.Property(x => x.ExecutionResultJson)
                .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => x.RequestedByUserId);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.TargetSystem);
            builder.HasIndex(x => x.CreatedAtUtc);
        }
    }
}