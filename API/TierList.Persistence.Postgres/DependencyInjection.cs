using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TierList.Domain.Abstraction;
using TierList.Domain.Repos;
using TierList.Persistence.Postgres.Repos;

namespace TierList.Persistence.Postgres;

/// <summary>
/// Provides extension methods for configuring persistence-related services in a dependency injection container.
/// </summary>
/// <remarks>This class contains methods to register database contexts, repositories, and other
/// persistence-related services into an <see cref="IServiceCollection"/>. It is designed to simplify the setup of
/// persistence layers in applications using dependency injection.</remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Configures and registers persistence-related services for the application.
    /// </summary>
    /// <remarks>This method registers the application's database context, unit of work, and repository
    /// services with the dependency injection container. It uses the connection string named "DefaultConnection" from
    /// the provided configuration to configure the database context.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the persistence services will be added.</param>
    /// <param name="config">The <see cref="IConfiguration"/> instance used to retrieve configuration settings, such as the database
    /// connection string.</param>
    public static void AddPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<TierListDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ITierListRepository, TierListRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }

    public static void EnsureDataInitialized(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TierListDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            dbContext.Database.Migrate();
        }
    }
}
