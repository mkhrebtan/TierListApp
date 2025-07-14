using System.Security.Claims;
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
        app.MapGet("/lists", async (HttpContext context, IQueryHandler<GetTierListsQuery, IReadOnlyCollection<TierListBriefDto>> queryHandler)
            => await GetTierLists(context, queryHandler)).RequireAuthorization();

        app.MapGet("/lists/{listId}", async (HttpContext context, int listId, IQueryHandler<GetTierListDataQuery, TierListDataDto> queryHandler)
            => await GetTierListData(context, listId, queryHandler)).RequireAuthorization();

        app.MapPost("/lists", async (HttpContext context, CreateTierListCommand request, ICommandHandler<CreateTierListCommand, TierListBriefDto> commandHandler)
            => await CreateTierList(context, request, commandHandler)).RequireAuthorization();

        app.MapPut("lists/{listId}", async (HttpContext context, int listId, UpdateTierListCommand request, ICommandHandler<UpdateTierListCommand, TierListBriefDto> commandHandler)
            => await UpdateTierList(context, listId, request, commandHandler)).RequireAuthorization();

        app.MapDelete("/lists/{listId}", async (HttpContext context, int listId, ICommandHandler<DeleteTierListCommand> commandHandler)
            => await DeleteTierList(context, listId, commandHandler)).RequireAuthorization();
    }

    private static async Task<IResult> GetTierLists(
        HttpContext context,
        IQueryHandler<GetTierListsQuery,
        IReadOnlyCollection<TierListBriefDto>> queryHandler)
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
        IQueryHandler<GetTierListDataQuery, TierListDataDto> queryHandler)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              context.User.FindFirst("sub") ??
                              context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        GetTierListDataQuery query = new GetTierListDataQuery { Id = listId, UserId = userId };
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
        ICommandHandler<CreateTierListCommand, TierListBriefDto> commandHandler)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              context.User.FindFirst("sub") ??
                              context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        command.UserId = userId;
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
        ICommandHandler<UpdateTierListCommand, TierListBriefDto> commandHandler)
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
        ICommandHandler<DeleteTierListCommand> commandHandler)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                          context.User.FindFirst("sub") ??
                          context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        var result = await commandHandler.Handle(new DeleteTierListCommand { Id = listId, UserId = userId });
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.NoContent();
    }
}