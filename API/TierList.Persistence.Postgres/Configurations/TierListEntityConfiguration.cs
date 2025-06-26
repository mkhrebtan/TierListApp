using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Domain.Entities;

namespace TierList.Persistence.Postgres.Configurations;

public class TierListEntityConfiguration : IEntityTypeConfiguration<TierListEntity>
{
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
