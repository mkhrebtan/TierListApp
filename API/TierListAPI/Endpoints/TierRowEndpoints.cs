using TierList.Application.Commands.TierRow;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class TierRowEndpoints
{
    internal static void MapTierRowEndpoints(this WebApplication app)
    {
        app.MapPost("/rows", async (CreateTierRowCommand request, ITierListService service)
            => await CreateTierRow(request, service)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/rank", async (int rowId, UpdateTierRowRankCommand request, ITierListService service)
            => await UpdateTierRowRank(rowId, request, service)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/color", async (int rowId, UpdateTierRowColorCommand request, ITierListService service)
            => await UpdateTierRowColor(rowId, request, service)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/order", async (int rowId, UpdateTierRowOrderCommand request, ITierListService service)
            => await UpdateTierRowOrder(rowId, request, service)).RequireAuthorization();

        app.MapDelete("/rows/{rowId}", async (int rowId, int listId, bool isDeleteWithImages, ITierListService service)
            => await DeleteTierRow(rowId, listId, isDeleteWithImages, service)).RequireAuthorization();
    }

    private static async Task<IResult> CreateTierRow(CreateTierRowCommand request, ITierListService service)
    {
        TierRowResult commandResult = await service.CreateTierRowAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return Results.Created($"/lists/{commandResult.TierRowData?.Id}", commandResult.TierRowData);
    }

    private static async Task<IResult> UpdateTierRowRank(int rowId, UpdateTierRowRankCommand request, ITierListService service)
    {
        if (rowId != request.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

        TierRowResult commandResult = await service.UpdateTierRowAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierRowData);
    }

    private static async Task<IResult> UpdateTierRowColor(int rowId, UpdateTierRowColorCommand request, ITierListService service)
    {
        if (rowId != request.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

        TierRowResult commandResult = await service.UpdateTierRowAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierRowData);
    }

    private static async Task<IResult> UpdateTierRowOrder(int rowId, UpdateTierRowOrderCommand request, ITierListService service)
    {
        if (rowId != request.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

        TierRowResult commandResult = await service.UpdateTierRowAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierRowData);
    }

    private static async Task<IResult> DeleteTierRow(int rowId, int listId, bool isDeleteWithImages, ITierListService service)
    {
        TierRowResult commandResult = await service.DeleteTierRowAsync(new DeleteTierRowCommand { Id = rowId, ListId = listId, IsDeleteWithImages = isDeleteWithImages });
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok();
    }
}
