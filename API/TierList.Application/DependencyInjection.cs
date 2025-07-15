using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TierList.Application.Commands.JwtUser.Login;
using TierList.Application.Commands.JwtUser.RefreshToken;
using TierList.Application.Commands.JwtUser.Register;
using TierList.Application.Commands.TierImage.Delete;
using TierList.Application.Commands.TierImage.Move;
using TierList.Application.Commands.TierImage.Reorder;
using TierList.Application.Commands.TierImage.Save;
using TierList.Application.Commands.TierImage.UpdateNote;
using TierList.Application.Commands.TierImage.UpdateUrl;
using TierList.Application.Commands.TierList.Create;
using TierList.Application.Commands.TierList.Delete;
using TierList.Application.Commands.TierList.Update;
using TierList.Application.Commands.TierRow;
using TierList.Application.Commands.TierRow.Create;
using TierList.Application.Commands.TierRow.Delete;
using TierList.Application.Commands.TierRow.UpdateOrder;
using TierList.Application.Commands.TierRow.UpdateRank;
using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Services;
using TierList.Application.Queries.GetListData;
using TierList.Application.Queries.GetLists;
using TierList.Application.Queries.GetUploadUrl;

namespace TierList.Application;

/// <summary>
/// Provides extension methods for registering application services with the dependency injection container.
/// </summary>
/// <remarks>This class contains methods to simplify the registration of application-specific services  into an
/// <see cref="IServiceCollection"/> for dependency injection.</remarks>
public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();

        // Commands
        RegisterHandlers(services);

        // Validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (type.IsClass && !type.IsAbstract)
            {
                var interfaces = type.GetInterfaces();

                foreach (var interfaceType in interfaces)
                {
                    if (interfaceType.IsGenericType)
                    {
                        var genericDefinition = interfaceType.GetGenericTypeDefinition();

                        if (genericDefinition == typeof(ICommandHandler<>) ||
                            genericDefinition == typeof(ICommandHandler<,>) ||
                            genericDefinition == typeof(IQueryHandler<,>))
                        {
                            services.AddScoped(interfaceType, type);
                        }
                    }
                }
            }
        }
    }
}
