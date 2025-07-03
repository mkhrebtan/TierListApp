using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TierList.Domain.Entities;

namespace TierList.Persistence.Postgres.Configurations;

/// <summary>
/// Configures the entity type settings for <see cref="TierImageEntity"/> in the database context.
/// </summary>
/// <remarks>This configuration maps the <see cref="TierImageEntity"/> to the "Image" table in the database, sets
/// up primary key and property constraints, and defines relationships with other entities.</remarks>
internal class TierImageEntityConfiguration : IEntityTypeConfiguration<TierImageEntity>
{
    /// <summary>
    /// Configures the entity type for <see cref="TierImageEntity"/> in the database context.
    /// </summary>
    /// <remarks>This method sets up the table name, primary key, property configurations, and relationships 
    /// for the <see cref="TierImageEntity"/>. The table is named "Image", the primary key is the  <c>Id</c> property,
    /// and the <c>Url</c> property is required. The <c>Note</c> property has a  default value of an empty string.
    /// Additionally, a one-to-many relationship is configured  between the <c>Container</c> entity and its associated
    /// images.</remarks>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TierImageEntity}"/> used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TierImageEntity> builder)
    {
        builder.ToTable("Image");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.StorageKey)
            .IsRequired();

        builder.Property(i => i.Url)
            .IsRequired();

        builder.Property(i => i.Note)
            .HasDefaultValue(string.Empty);

        builder.HasOne(i => i.Container)
            .WithMany(c => c.Images)
            .HasForeignKey(i => i.ContainerId);
    }
}
