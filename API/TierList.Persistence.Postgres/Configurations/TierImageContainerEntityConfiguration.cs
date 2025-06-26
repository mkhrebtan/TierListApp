using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TierList.Domain.Entities;

namespace TierList.Persistence.Postgres.Configurations;

internal class TierImageContainerEntityConfiguration : IEntityTypeConfiguration<TierImageContainer>
{
    public void Configure(EntityTypeBuilder<TierImageContainer> builder)
    {
        builder.ToTable("Container");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.HasDiscriminator<string>("ContainerType")
            .HasValue<TierRowEntity>("TierRow")
            .HasValue<TierBackupRowEntity>("TierBackupRow");

        builder.HasOne(c => c.TierList)
            .WithMany(t => t.Containers)
            .HasForeignKey(c => c.TierListId);

        builder.HasMany(c => c.Images)
            .WithOne(i => i.Container)
            .HasForeignKey(i => i.ContainerId);
    }
}
