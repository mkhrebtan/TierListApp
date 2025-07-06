using System.Security.Claims;
using TierList.Application.Commands.TierList;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierList.Application.Queries;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class TierListEndpoints
{
    internal static void MapTierListEndpoints(this WebApplication app)
    {
        app.MapGet("/lists", async (HttpContext context, ITierListService service)
            => await GetTierLists(context, service)).RequireAuthorization();

        app.MapGet("/lists/{listId}", async (HttpContext context, int listId, ITierListService service)
            => await GetTierListData(context, listId, service)).RequireAuthorization();

        app.MapPost("/lists", async (HttpContext context, CreateTierListCommand request, ITierListService service)
            => await CreateTierList(context, request, service)).RequireAuthorization();

        app.MapPut("lists/{listId}", async (HttpContext context, int listId, UpdateTierListCommand request, ITierListService service) 
            => await UpdateTierList(context, listId, request, service)).RequireAuthorization();

        app.MapDelete("/lists/{listId}", async (HttpContext context, int listId, ITierListService service) 
            => await DeleteTierList(context, listId, service)).RequireAuthorization();
    }

    private static async Task<IResult> GetTierLists(HttpContext context, ITierListService service)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? context.User.FindFirst("sub");

        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        var query = new GetTierListsQuery { UserId = userId };
        var result = await service.GetTierListsAsync(query);

        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Ok(result.TierListsData);
    }

    private static async Task<IResult> GetTierListData(HttpContext context, int listId, ITierListService service)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              context.User.FindFirst("sub") ??
                              context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        GetTierListDataQuery query = new GetTierListDataQuery { Id = listId, UserId = userId };
        TierListResult result = await service.GetTierListDataAsync(query);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Ok(result.TierListData);
    }

    private static async Task<IResult> CreateTierList(HttpContext context, CreateTierListCommand request, ITierListService service)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                              context.User.FindFirst("sub") ??
                              context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        request.UserId = userId;
        TierListResult commandResult = await service.CreateTierListAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return Results.Created($"/lists/{commandResult.TierListData?.Id}", commandResult.TierListData);
    }

    private static async Task<IResult> UpdateTierList(HttpContext context, int listId, UpdateTierListCommand request, ITierListService service)
    {
        if (listId != request.Id)
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

        request.UserId = userId;
        TierListResult commandResult = await service.UpdateTierListAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierListData);
    }

    private static async Task<IResult> DeleteTierList(HttpContext context, int listId, ITierListService service)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) ??
                          context.User.FindFirst("sub") ??
                          context.User.FindFirst("userId");

        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return TypedResults.Unauthorized();
        }

        TierListResult commandResult = await service.DeleteTierListAsync(new DeleteTierListCommand { Id = listId, UserId = userId });
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.NoContent();
    }
}