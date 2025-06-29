using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TierList.Domain.Entities;

namespace TierList.Persistence.Postgres.Configurations;

/// <summary>
/// Configures the entity type settings for <see cref="TierListEntity"/> in the database context.
/// </summary>
/// <remarks>This configuration maps the <see cref="TierListEntity"/> to the "List" table in the database, sets up
/// the primary key, and defines the relationship between <see cref="TierListEntity"/> and its containers.</remarks>
public class TierListEntityConfiguration : IEntityTypeConfiguration<TierListEntity>
{
    /// <summary>
    /// Configures the entity type for the <see cref="TierListEntity"/> class.
    /// </summary>
    /// <remarks>This method configures the database table name, primary key, and relationships for the  <see
    /// cref="TierListEntity"/> entity. The table is mapped to "List", the primary key is  configured to auto-generate
    /// values, and a one-to-many relationship is established with  the containers associated with the tier
    /// list.</remarks>
    /// <param name="builder">An <see cref="EntityTypeBuilder{TEntity}"/> instance used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TierListEntity> builder)
    {
        builder.ToTable("List");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.HasMany(t => t.Containers)
            .WithOne(c => c.TierList)
            .HasForeignKey(c => c.TierListId);
    }
}
