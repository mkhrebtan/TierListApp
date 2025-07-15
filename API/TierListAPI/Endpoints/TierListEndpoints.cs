using System.Security.Claims;
using FluentValidation;
using TierList.Application.Commands.TierList.Create;
using TierList.Application.Commands.TierList.Delete;
using TierList.Application.Commands.TierList.Update;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;
using TierList.Application.Queries.GetListData;
using TierList.Application.Queries.GetLists;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class TierListEndpoints
{
    internal static void MapTierListEndpoints(this WebApplication app)
    {
        app.MapGet("/lists", async (
            HttpContext context,
            IQueryHandler<GetTierListsQuery, IReadOnlyCollection<TierListBriefDto>> queryHandler) => await GetTierLists(context, queryHandler)).RequireAuthorization();

        app.MapGet("/lists/{listId}", async (
            HttpContext context,
            int listId,
            IQueryHandler<GetTierListDataQuery, TierListDataDto> queryHandler,
            IValidator<GetTierListDataQuery> validator) => await GetTierListData(context, listId, queryHandler, validator)).RequireAuthorization();

        app.MapPost("/lists", async (
            HttpContext context,
            CreateTierListCommand request,
            ICommandHandler<CreateTierListCommand, TierListBriefDto> commandHandler,
            IValidator<CreateTierListCommand> validator) => await CreateTierList(context, request, commandHandler, validator)).RequireAuthorization();

        app.MapPut("lists/{listId}", async (
            HttpContext context,
            int listId,
            UpdateTierListCommand request,
            ICommandHandler<UpdateTierListCommand, TierListBriefDto> commandHandler,
            IValidator<UpdateTierListCommand> validator) => await UpdateTierList(context, listId, request, commandHandler, validator)).RequireAuthorization();

        app.MapDelete("/lists/{listId}", async (
            HttpContext context,
            int listId,
            ICommandHandler<DeleteTierListCommand> commandHandler,
            IValidator<DeleteTierListCommand> validator) => await DeleteTierList(context, listId, commandHandler, validator)).RequireAuthorization();
    }

    private static async Task<IResult> GetTierLists(
        HttpContext context,
        IQueryHandler<GetTierListsQuery, IReadOnlyCollection<TierListBriefDto>> queryHandler)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? context.User.FindFirst("sub");

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        var query = new GetTierListsQuery { UserId = userId };
        var result = await queryHandler.Handle(query);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> GetTierListData(
        HttpContext context,
        int listId,
        IQueryHandler<GetTierListDataQuery, TierListDataDto> queryHandler,
        IValidator<GetTierListDataQuery> validator)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              context.User.FindFirst("sub") ??
                              context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        GetTierListDataQuery query = new GetTierListDataQuery { Id = listId, UserId = userId };
        var validationResult = await validator.ValidateAsync(query);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await queryHandler.Handle(query);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> CreateTierList(
        HttpContext context,
        CreateTierListCommand command,
        ICommandHandler<CreateTierListCommand, TierListBriefDto> commandHandler,
        IValidator<CreateTierListCommand> validator)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              context.User.FindFirst("sub") ??
                              context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        command.UserId = userId;
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

        return Results.Created($"/lists/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateTierList(
        HttpContext context,
        int listId,
        UpdateTierListCommand command,
        ICommandHandler<UpdateTierListCommand, TierListBriefDto> commandHandler,
        IValidator<UpdateTierListCommand> validator)
    {
        if (listId != command.Id)
        {
            return TypedResults.BadRequest("List ID in the URL does not match the ID in the request body.");
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                      context.User.FindFirst("sub") ??
                      context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        command.UserId = userId;
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

    private static async Task<IResult> DeleteTierList(
        HttpContext context,
        int listId,
        ICommandHandler<DeleteTierListCommand> commandHandler,
        IValidator<DeleteTierListCommand> validator)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                          context.User.FindFirst("sub") ??
                          context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        var command = new DeleteTierListCommand { Id = listId, UserId = userId };
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

        return TypedResults.NoContent();
    }
}