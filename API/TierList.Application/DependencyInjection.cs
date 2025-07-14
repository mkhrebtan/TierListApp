using Microsoft.Extensions.DependencyInjection;
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

        // TierList
        services.AddScoped<ICommandHandler<CreateTierListCommand, TierListBriefDto>, CreateTierListCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTierListCommand, TierListBriefDto>, UpdateTierListCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTierListCommand>, DeleteTierListCommandHandler>();

        // TierRow
        services.AddScoped<ICommandHandler<CreateTierRowCommand, TierRowBriefDto>, CreateTierRowCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTierRowColorCommand, TierRowBriefDto>, UpdateTierRowColorCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTierRowOrderCommand, TierRowBriefDto>, UpdateTierRowOrderCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTierRowRankCommand, TierRowBriefDto>, UpdateTierRowRankCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTierRowCommand>, DeleteTierRowCommandHandler>();

        // TierImage
        services.AddScoped<ICommandHandler<SaveTierImageCommand, TierImageDto>, SaveTierImageCommandHandler>();
        services.AddScoped<ICommandHandler<ReorderTierImageCommand, TierImageDto>, ReorderTierImageCommandHandler>();
        services.AddScoped<ICommandHandler<MoveTierImageCommand, TierImageDto>, MoveTierImageCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTierImageNoteCommand, TierImageDto>, UpdateTierImageNoteCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTierImageUrlCommand, TierImageDto>, UpdateTierImageUrlCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTierImageCommand>, DeleteTierImageCommandHandler>();

        // User
        services.AddScoped<ICommandHandler<RegisterUserCommand, RegisterUserDto>, RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<LoginUserCommand, LoginUserDto>, LoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<RefreshTokenCommand, LoginUserDto>, RefreshTokenCommandHandler>();

        // Queries

        // TierList
        services.AddScoped<IQueryHandler<GetTierListsQuery, IReadOnlyCollection<TierListBriefDto>>, GetTierListsQueryHandler>();
        services.AddScoped<IQueryHandler<GetTierListDataQuery, TierListDataDto>, GetTierListDataQueryHandler>();

        // TierImage
        services.AddScoped<IQueryHandler<GetTierImageUploadUrlQuery, TierImageBriefDto>, GetTierImageUploadUrlQueryHandler>();
    }
}
