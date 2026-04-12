namespace AiOperationsHub.Persistence.Configurations
{
    using AiOperationsHub.Persistence.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Configures <see cref="SystemPromptSettingDbEntity"/>.
    /// </summary>
    public sealed class SystemPromptSettingConfiguration
        : IEntityTypeConfiguration<SystemPromptSettingDbEntity>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SystemPromptSettingDbEntity> builder)
        {
            builder.ToTable("SystemPromptSettings");

            builder.HasKey(x => x.Key);

            builder.Property(x => x.Key)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(x => x.Value)
                .HasMaxLength(20000)
                .IsRequired();

            builder.Property(x => x.UpdatedUtc)
                .IsRequired();
        }
    }
}