using TierList.Application.Commands.TierRow;
using TierList.Application.Commands.TierRow.Create;
using TierList.Application.Commands.TierRow.Delete;
using TierList.Application.Commands.TierRow.UpdateOrder;
using TierList.Application.Commands.TierRow.UpdateRank;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class TierRowEndpoints
{
    internal static void MapTierRowEndpoints(this WebApplication app)
    {
        app.MapPost("/rows", async (CreateTierRowCommand request, ICommandHandler<CreateTierRowCommand, TierRowBriefDto> commandHandler)
            => await CreateTierRow(request, commandHandler)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/rank", async (int rowId, UpdateTierRowRankCommand request, ICommandHandler<UpdateTierRowRankCommand, TierRowBriefDto> commandHandler)
            => await UpdateTierRowRank(rowId, request, commandHandler)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/color", async (int rowId, UpdateTierRowColorCommand request, ICommandHandler<UpdateTierRowColorCommand, TierRowBriefDto> commandHandler)
            => await UpdateTierRowColor(rowId, request, commandHandler)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/order", async (int rowId, UpdateTierRowOrderCommand request, ICommandHandler<UpdateTierRowOrderCommand, TierRowBriefDto> commandHandler)
            => await UpdateTierRowOrder(rowId, request, commandHandler)).RequireAuthorization();

        app.MapDelete("/rows/{rowId}", async (int rowId, int listId, bool isDeleteWithImages, ICommandHandler<DeleteTierRowCommand> commandHandler)
            => await DeleteTierRow(rowId, listId, isDeleteWithImages, commandHandler)).RequireAuthorization();
    }

    private static async Task<IResult> CreateTierRow(
        CreateTierRowCommand command,
        ICommandHandler<CreateTierRowCommand, TierRowBriefDto> commandHandler)
    {
        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return Results.Created($"/lists/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateTierRowRank(
        int rowId,
        UpdateTierRowRankCommand command,
        ICommandHandler<UpdateTierRowRankCommand, TierRowBriefDto> commandHandler)
    {
        if (rowId != command.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> UpdateTierRowColor(
        int rowId,
        UpdateTierRowColorCommand command,
        ICommandHandler<UpdateTierRowColorCommand, TierRowBriefDto> commandHandler)
    {
        if (rowId != command.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> UpdateTierRowOrder(
        int rowId,
        UpdateTierRowOrderCommand command,
        ICommandHandler<UpdateTierRowOrderCommand, TierRowBriefDto> commandHandler)
    {
        if (rowId != command.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> DeleteTierRow(
        int rowId,
        int listId,
        bool isDeleteWithImages,
        ICommandHandler<DeleteTierRowCommand> commandHandler)
    {
        var result = await commandHandler.Handle(new DeleteTierRowCommand { Id = rowId, ListId = listId, IsDeleteWithImages = isDeleteWithImages });
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok();
    }
}
