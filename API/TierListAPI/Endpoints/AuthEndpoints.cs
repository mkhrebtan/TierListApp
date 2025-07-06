using TierList.Application.Commands.User;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class AuthEndpoints
{
    internal static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async (RegisterUserCommand request, IUserService service)
            => await RegisterUser(request, service));

        app.MapPost("/auth/login", async (LoginUserCommand request, IUserService service)
            => await LoginUser(request, service));

        app.MapPost("auth/refresh", async (RefreshTokenCommand request, IUserService service)
            => await RefreshToken(request, service));
    }

    private static async Task<IResult> RegisterUser(RegisterUserCommand request, IUserService service)
    {
        RegisterUserResult result = await service.RegisterUserAsync(request);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Created("/auth/login", result.RegisterUserDto);
    }

    private static async Task<IResult> LoginUser(LoginUserCommand request, IUserService service)
    {
        LoginUserResult result = await service.LoginUserAsync(request);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Ok(result.LoginUserDto);
    }

    private static async Task<IResult> RefreshToken(RefreshTokenCommand request, IUserService service)
    {
        LoginUserResult result = await service.RefreshTokenAsync(request);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Ok(result.LoginUserDto);
    }
}