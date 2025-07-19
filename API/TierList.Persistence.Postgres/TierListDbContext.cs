using Microsoft.EntityFrameworkCore;
using TierList.Domain.Entities;
using TierList.Persistence.Postgres.Configurations;

namespace TierList.Persistence.Postgres;

/// <summary>
/// Represents the database context for managing tier lists and related entities in the application.
/// </summary>
/// <remarks>This context is used to interact with the database for operations involving tier lists,  tier image
/// containers, and tier images. It provides DbSet properties for querying and saving  instances of these entities. The
/// context is configured to use PostgreSQL as the database provider.</remarks>
public class TierListDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TierListDbContext"/> class.
    /// </summary>
    /// <remarks>This constructor creates a new instance of the database context, which is typically used to
    /// interact with the underlying database. Use this constructor when configuring the context with default
    /// options.</remarks>
    public TierListDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TierListDbContext"/> class with the specified options.
    /// </summary>
    /// <remarks>Use this constructor to configure and initialize the <see cref="TierListDbContext"/> with the
    /// desired options, such as database provider and connection details. The options are passed to the base <see
    /// cref="DbContext"/> class.</remarks>
    /// <param name="options">The options to configure the database context. This typically includes the connection string and other
    /// database-specific settings.</param>
    public TierListDbContext(DbContextOptions<TierListDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of tier lists stored in the database.
    /// </summary>
    public DbSet<TierListEntity> TierLists { get; set; }

    /// <summary>
    /// Gets or sets the database set of tier image containers.
    /// </summary>
    public DbSet<TierImageContainer> TierImageContainers { get; set; }

    /// <summary>
    /// Gets or sets the database set of tier images.
    /// </summary>
    public DbSet<TierImageEntity> TierImages { get; set; }

    /// <summary>
    /// Gets or sets the collection of users in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the database set of refresh tokens.
    /// </summary>
    /// <remarks>This property provides access to the refresh tokens stored in the database.  It can be used
    /// to query, add, update, or delete refresh token records.</remarks>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    /// <summary>
    /// Configures the model for the database context by applying entity configurations.
    /// </summary>
    /// <remarks>This method applies specific configurations for the entities in the context by using the <see
    /// cref="IEntityTypeConfiguration{TEntity}"/> implementations. It also calls the base implementation to ensure any
    /// additional configuration defined in the base class is applied.</remarks>
    /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure the entity mappings and relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TierListEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TierImageContainerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TierRowEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TierImageEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures the database context options for this instance.
    /// </summary>
    /// <remarks>This method is called during the initialization of the database context to configure options
    /// such as the database provider. By default, this implementation configures the context to use the PostgreSQL
    /// database provider.</remarks>
    /// <param name="optionsBuilder">A builder used to configure the database context options. This parameter cannot be null.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }
}
