using Microsoft.EntityFrameworkCore;
using TierList.Domain.Entities;
using TierList.Persistence.Postgres.Configurations;

namespace TierList.Persistence.Postgres;

public class TierListDbContext : DbContext
{
    public DbSet<TierListEntity> TierLists { get; set; }
    public DbSet<TierImageContainer> TierImageContainers { get; set; }
    public DbSet<TierImageEntity> TierImages { get; set; }

    public TierListDbContext() { }

    public TierListDbContext(DbContextOptions<TierListDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TierListEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TierImageContainerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TierImageEntityConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
