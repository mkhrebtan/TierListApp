using FluentValidation;
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
        app.MapPost("/auth/register", async (
            RegisterUserCommand request,
            ICommandHandler<RegisterUserCommand, RegisterUserDto> commandHandler,
            IValidator<RegisterUserCommand> validator) => await RegisterUser(request, commandHandler, validator));

        app.MapPost("/auth/login", async (
            LoginUserCommand request,
            ICommandHandler<LoginUserCommand, LoginUserDto> commandHandler,
            IValidator<LoginUserCommand> validator) => await LoginUser(request, commandHandler, validator));

        app.MapPost("auth/refresh", async (
            RefreshTokenCommand request,
            ICommandHandler<RefreshTokenCommand, LoginUserDto> commandHandler,
            IValidator<RefreshTokenCommand> validator) => await RefreshToken(request, commandHandler, validator));
    }

    private static async Task<IResult> RegisterUser(
        RegisterUserCommand command,
        ICommandHandler<RegisterUserCommand, RegisterUserDto> commandHandler,
        IValidator<RegisterUserCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Created("/auth/register", result.Value);
    }

    private static async Task<IResult> LoginUser(
        LoginUserCommand command,
        ICommandHandler<LoginUserCommand, LoginUserDto> commandHandler,
        IValidator<LoginUserCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> RefreshToken(
        RefreshTokenCommand command,
        ICommandHandler<RefreshTokenCommand, LoginUserDto> commandHandler,
        IValidator<RefreshTokenCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }
}