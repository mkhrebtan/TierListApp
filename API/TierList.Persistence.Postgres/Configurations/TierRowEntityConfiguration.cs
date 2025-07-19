using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TierList.Domain.Entities;
using TierList.Domain.ValueObjects;

namespace TierList.Persistence.Postgres.Configurations;

/// <summary>
/// Configures the database schema for the <see cref="TierRowEntity"/> type.
/// </summary>
/// <remarks>This configuration sets up the properties of the <see cref="TierRowEntity"/> with specific
/// constraints and conversions to ensure data integrity and proper storage in the database.</remarks>
internal class TierRowEntityConfiguration : IEntityTypeConfiguration<TierRowEntity>
{
    /// <summary>
    /// Configures the properties of the <see cref="TierRowEntity"/> entity type.
    /// </summary>
    /// <remarks>This method sets up the required properties and constraints for the <see
    /// cref="TierRowEntity"/> entity. The <c>Rank</c> property is required and has a maximum length defined by <see
    /// cref="TierRowEntity.MaxRankLength"/>. The <c>ColorHex</c> property is required and is stored as non-Unicode. The
    /// <c>Order</c> property is required and uses a custom conversion for storage.</remarks>
    /// <param name="builder">The builder used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<TierRowEntity> builder)
    {
        builder.Property(r => r.Rank)
            .IsRequired()
            .HasMaxLength(TierRowEntity.MaxRankLength);

        builder.Property(r => r.ColorHex)
            .IsRequired()
            .IsUnicode(false);

        builder.Property(r => r.Order).HasConversion(
            order => order.Value,
            value => Order.Create(value).Value).IsRequired();
    }
}
