using TierList.Application.Commands.JwtUser.Login;
using TierList.Application.Commands.JwtUser.RefreshToken;
using TierList.Application.Commands.JwtUser.Register;
using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class AuthEndpoints
{
    internal static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async (RegisterUserCommand request, ICommandHandler<RegisterUserCommand, RegisterUserDto> commandHandler)
            => await RegisterUser(request, commandHandler));

        app.MapPost("/auth/login", async (LoginUserCommand request, ICommandHandler<LoginUserCommand, LoginUserDto> commandHandler)
            => await LoginUser(request, commandHandler));

        app.MapPost("auth/refresh", async (RefreshTokenCommand request, ICommandHandler<RefreshTokenCommand, LoginUserDto> commandHandler)
            => await RefreshToken(request, commandHandler));
    }

    private static async Task<IResult> RegisterUser(
        RegisterUserCommand command,
        ICommandHandler<RegisterUserCommand, RegisterUserDto> commandHandler)
    {
        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Created("/auth/register", result.Value);
    }

    private static async Task<IResult> LoginUser(
        LoginUserCommand command,
        ICommandHandler<LoginUserCommand, LoginUserDto> commandHandler)
    {
        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenCommand command,
        ICommandHandler<RefreshTokenCommand, LoginUserDto> commandHandler)
    {
        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }
}