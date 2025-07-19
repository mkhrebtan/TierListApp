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
    /// Configures the entity of type <see cref="TierListEntity"/> in the database context.
    /// </summary>
    /// <remarks>This method sets up the table mapping, primary key, and relationships for the <see
    /// cref="TierListEntity"/>. It specifies the table name, configures the primary key to be auto-generated, and
    /// enforces constraints on the <c>Title</c> property. Additionally, it establishes relationships with the
    /// <c>Containers</c> collection and the <c>User</c> entity.</remarks>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TierListEntity> builder)
    {
        builder.ToTable("List");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(TierListEntity.MaxTitleLength);

        builder.HasMany(t => t.Containers)
            .WithOne()
            .HasForeignKey(c => c.TierListId);

        builder.HasOne<User>()
            .WithMany(u => u.TierLists)
            .HasForeignKey(t => t.UserId);
    }
}