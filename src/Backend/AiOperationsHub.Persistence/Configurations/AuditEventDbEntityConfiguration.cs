using AiOperationsHub.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AiOperationsHub.Persistence.Configurations
{
    /// <summary>
    /// Configures the EF Core mapping for <see cref="AuditEventDbEntity"/>.
    /// </summary>
    public sealed class AuditEventDbEntityConfiguration : IEntityTypeConfiguration<AuditEventDbEntity>
    {
        /// <summary>
        /// Configures the database mapping for the audit event entity.
        /// </summary>
        /// <param name="builder">The entity type builder.</param>
        public void Configure(EntityTypeBuilder<AuditEventDbEntity> builder)
        {
            builder.ToTable("AuditEvents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CorrelationId)
                .IsRequired();

            builder.Property(x => x.EventType)
                .IsRequired();

            builder.Property(x => x.Verbosity)
                .IsRequired();

            builder.Property(x => x.Result)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.SourceSystem)
                .HasMaxLength(200);

            builder.Property(x => x.TargetResource)
                .HasMaxLength(500);

            builder.Property(x => x.PayloadJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(x => x.CorrelationId);
            builder.HasIndex(x => x.ConversationId);
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.EventType);
            builder.HasIndex(x => x.CreatedAtUtc);
        }
    }
}