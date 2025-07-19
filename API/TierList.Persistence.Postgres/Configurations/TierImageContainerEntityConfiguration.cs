using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TierList.Domain.Entities;

namespace TierList.Persistence.Postgres.Configurations;

/// <summary>
/// Configures the entity type <see cref="TierImageContainer"/> for use with Entity Framework Core.
/// </summary>
/// <remarks>This configuration maps the <see cref="TierImageContainer"/> entity to the "Container" table in the
/// database. It defines the primary key, a discriminator for inheritance, and relationships with other
/// entities.</remarks>
internal class TierImageContainerEntityConfiguration : IEntityTypeConfiguration<TierImageContainer>
{
    /// <summary>
    /// Configures the entity type for <see cref="TierImageContainer"/>.
    /// </summary>
    /// <remarks>This method configures the table name, primary key, discriminator for inheritance, and
    /// relationships for the <see cref="TierImageContainer"/> entity. It maps the entity to the "Container" table, sets
    /// up a discriminator column for derived types, and defines relationships with the <see cref="TierList"/> and <see
    /// cref="TierImage"/> entities.</remarks>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the <see cref="TierImageContainer"/> entity.</param>
    public void Configure(EntityTypeBuilder<TierImageContainer> builder)
    {
        builder.ToTable("Container");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.HasDiscriminator<string>("ContainerType")
            .HasValue<TierRowEntity>("TierRow")
            .HasValue<TierBackupRowEntity>("TierBackupRow");

        builder.HasOne<TierListEntity>()
            .WithMany(t => t.Containers)
            .HasForeignKey(c => c.TierListId);

        builder.HasMany(c => c.Images)
            .WithOne()
            .HasForeignKey(i => i.ContainerId);
    }
}