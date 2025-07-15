using FluentValidation;
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
        app.MapPost("/rows", async (
            CreateTierRowCommand request,
            ICommandHandler<CreateTierRowCommand, TierRowBriefDto> commandHandler,
            IValidator<CreateTierRowCommand> validator) => await CreateTierRow(request, commandHandler, validator)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/rank", async (
            int rowId,
            UpdateTierRowRankCommand request,
            ICommandHandler<UpdateTierRowRankCommand, TierRowBriefDto> commandHandler,
            IValidator<UpdateTierRowRankCommand> validator) => await UpdateTierRowRank(rowId, request, commandHandler, validator)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/color", async (
            int rowId,
            UpdateTierRowColorCommand request,
            ICommandHandler<UpdateTierRowColorCommand, TierRowBriefDto> commandHandler,
            IValidator<UpdateTierRowColorCommand> validator) => await UpdateTierRowColor(rowId, request, commandHandler, validator)).RequireAuthorization();

        app.MapPut("/rows/{rowId}/order", async (
            int rowId,
            UpdateTierRowOrderCommand request,
            ICommandHandler<UpdateTierRowOrderCommand, TierRowBriefDto> commandHandler,
            IValidator<UpdateTierRowOrderCommand> validator) => await UpdateTierRowOrder(rowId, request, commandHandler, validator)).RequireAuthorization();

        app.MapDelete("/rows/{rowId}", async (
            int rowId,
            int listId,
            bool isDeleteWithImages,
            ICommandHandler<DeleteTierRowCommand> commandHandler,
            IValidator<DeleteTierRowCommand> validator) => await DeleteTierRow(rowId, listId, isDeleteWithImages, commandHandler, validator)).RequireAuthorization();
    }

    private static async Task<IResult> CreateTierRow(
        CreateTierRowCommand command,
        ICommandHandler<CreateTierRowCommand, TierRowBriefDto> commandHandler,
        IValidator<CreateTierRowCommand> validator)
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

        return Results.Created($"/lists/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateTierRowRank(
        int rowId,
        UpdateTierRowRankCommand command,
        ICommandHandler<UpdateTierRowRankCommand, TierRowBriefDto> commandHandler,
        IValidator<UpdateTierRowRankCommand> validator)
    {
        if (rowId != command.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

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

    private static async Task<IResult> UpdateTierRowColor(
        int rowId,
        UpdateTierRowColorCommand command,
        ICommandHandler<UpdateTierRowColorCommand, TierRowBriefDto> commandHandler,
        IValidator<UpdateTierRowColorCommand> validator)
    {
        if (rowId != command.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

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

    private static async Task<IResult> UpdateTierRowOrder(
        int rowId,
        UpdateTierRowOrderCommand command,
        ICommandHandler<UpdateTierRowOrderCommand, TierRowBriefDto> commandHandler,
        IValidator<UpdateTierRowOrderCommand> validator)
    {
        if (rowId != command.Id)
        {
            return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
        }

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

    private static async Task<IResult> DeleteTierRow(
        int rowId,
        int listId,
        bool isDeleteWithImages,
        ICommandHandler<DeleteTierRowCommand> commandHandler,
        IValidator<DeleteTierRowCommand> validator)
    {
        DeleteTierRowCommand command = new DeleteTierRowCommand { Id = rowId, ListId = listId, IsDeleteWithImages = isDeleteWithImages };

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

        return TypedResults.Ok();
    }
}
