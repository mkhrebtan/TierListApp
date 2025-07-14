using TierList.Application.Commands.TierImage.Delete;
using TierList.Application.Commands.TierImage.Move;
using TierList.Application.Commands.TierImage.Reorder;
using TierList.Application.Commands.TierImage.Save;
using TierList.Application.Commands.TierImage.UpdateNote;
using TierList.Application.Commands.TierImage.UpdateUrl;
using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Queries.GetUploadUrl;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class TierImageEndpoints
{
    internal static void MapTierImageEndpoints(this WebApplication app)
    {
        app.MapGet("/images/upload-url", async (string fileName, string contentType, IQueryHandler<GetTierImageUploadUrlQuery, TierImageBriefDto> queryHandler)
            => await GetImageUploadUrl(fileName, contentType, queryHandler)).RequireAuthorization();

        app.MapPost("/images", async (SaveTierImageCommand request, ICommandHandler<SaveTierImageCommand, TierImageDto> commandHandler)
            => await SaveTierImage(request, commandHandler)).RequireAuthorization();

        app.MapPut("/images/{imageId}/note", async (int imageId, UpdateTierImageNoteCommand request, ICommandHandler<UpdateTierImageNoteCommand, TierImageDto> commandHandler)
            => await UpdateTierImageNote(imageId, request, commandHandler)).RequireAuthorization();

        app.MapPut("/images/{imageId}/url", async (int imageId, UpdateTierImageUrlCommand request, ICommandHandler<UpdateTierImageUrlCommand, TierImageDto> commandHandler)
            => await UpdateTierImageUrl(imageId, request, commandHandler)).RequireAuthorization();

        app.MapPut("/images/{imageId}/reorder", async (int imageId, ReorderTierImageCommand request, ICommandHandler<ReorderTierImageCommand, TierImageDto> commandHandler)
            => await ReorderTierImage(imageId, request, commandHandler)).RequireAuthorization();

        app.MapPut("/images/{imageId}/move", async (int imageId, MoveTierImageCommand request, ICommandHandler<MoveTierImageCommand, TierImageDto> commandHandler)
            => await MoveTierImage(imageId, request, commandHandler)).RequireAuthorization();

        app.MapDelete("/images/{imageId}", async (int imageId, int listId, int containerId, ICommandHandler<DeleteTierImageCommand> commandHandler)
            => await DeleteTierImage(imageId, listId, containerId, commandHandler)).RequireAuthorization();
    }

    private static async Task<IResult> GetImageUploadUrl(
        string fileName,
        string contentType,
        IQueryHandler<GetTierImageUploadUrlQuery, TierImageBriefDto> queryHandler)
    {
        GetTierImageUploadUrlQuery request = new GetTierImageUploadUrlQuery { FileName = fileName, ContentType = contentType };
        var result = await queryHandler.Handle(request);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> SaveTierImage(SaveTierImageCommand command, ICommandHandler<SaveTierImageCommand, TierImageDto> commandHandler)
    {
        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Created($"/images/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateTierImageNote(
        int imageId,
        UpdateTierImageNoteCommand command,
        ICommandHandler<UpdateTierImageNoteCommand, TierImageDto> commandHandler)
    {
        if (imageId != command.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> UpdateTierImageUrl(
        int imageId,
        UpdateTierImageUrlCommand command,
        ICommandHandler<UpdateTierImageUrlCommand, TierImageDto> commandHandler)
    {
        if (imageId != command.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> ReorderTierImage(
        int imageId,
        ReorderTierImageCommand command,
        ICommandHandler<ReorderTierImageCommand, TierImageDto> commandHandler)
    {
        if (imageId != command.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> MoveTierImage(
        int imageId,
        MoveTierImageCommand command,
        ICommandHandler<MoveTierImageCommand, TierImageDto> commandHandler)
    {
        if (imageId != command.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        var result = await commandHandler.Handle(command);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok(result.Value);
    }

    private static async Task<IResult> DeleteTierImage(
        int imageId,
        int listId,
        int containerId,
        ICommandHandler<DeleteTierImageCommand> commandHandler)
    {
        var result = await commandHandler.Handle(new DeleteTierImageCommand { Id = imageId, ListId = listId, ContainerId = containerId });
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError(result.Error);
        }

        return TypedResults.Ok();
    }
}
