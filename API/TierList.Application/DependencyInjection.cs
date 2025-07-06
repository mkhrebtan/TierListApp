using Microsoft.Extensions.DependencyInjection;
using TierList.Application.Common.Services;

namespace TierList.Application;

/// <summary>
/// Provides extension methods for registering application services with the dependency injection container.
/// </summary>
/// <remarks>This class contains methods to simplify the registration of application-specific services  into an
/// <see cref="IServiceCollection"/> for dependency injection.</remarks>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application-specific services into the dependency injection container.
    /// </summary>
    /// <remarks>This method adds scoped services required by the application. Specifically, it registers the
    /// <see cref="ITierListService"/> interface with its implementation <see cref="TierListService"/>.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITierListService, TierListService>();
    }
}
