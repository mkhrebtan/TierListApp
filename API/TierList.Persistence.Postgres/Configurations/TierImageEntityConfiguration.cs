using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TierList.Domain.Entities;

namespace TierList.Persistence.Postgres.Configurations;

internal class TierImageEntityConfiguration : IEntityTypeConfiguration<TierImageEntity>
{
    public void Configure(EntityTypeBuilder<TierImageEntity> builder)
    {
        builder.ToTable("Image");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedOnAdd();

        builder.Property(i => i.Url)
            .IsRequired();

        builder.Property(i => i.Note)
            .HasDefaultValue(String.Empty);

        builder.HasOne(i => i.Container)
            .WithMany(c => c.Images)
            .HasForeignKey(i => i.ContainerId);
    }
}   
