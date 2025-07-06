using TierList.Application.Commands.TierImage;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierList.Application.Queries;
using TierListAPI.Helpers;

namespace TierListAPI.Endpoints;

internal static class TierImageEndpoints
{
    internal static void MapTierImageEndpoints(this WebApplication app)
    {
        app.MapGet("/images/upload-url", async (string fileName, string contentType, ITierListService service)
            => await GetImageUploadUrl(fileName, contentType, service)).RequireAuthorization();

        app.MapPost("/images", async (SaveTierImageCommand request, ITierListService service)
            => await SaveTierImage(request, service)).RequireAuthorization();

        app.MapPut("/images/{imageId}/note", async (int imageId, UpdateTierImageNoteCommand request, ITierListService service)
            => await UpdateTierImageNote(imageId, request, service)).RequireAuthorization();

        app.MapPut("/images/{imageId}/url", async (int imageId, UpdateTierImageUrlCommand request, ITierListService service)
            => await UpdateTierImageUrl(imageId, request, service)).RequireAuthorization();

        app.MapPut("/images/{imageId}/reorder", async (int imageId, ReorderTierImageCommand request, ITierListService service)
            => await ReorderTierImage(imageId, request, service)).RequireAuthorization();

        app.MapPut("/images/{imageId}/move", async (int imageId, MoveTierImageCommand request, ITierListService service)
            => await MoveTierImage(imageId, request, service)).RequireAuthorization();

        app.MapDelete("/images/{imageId}", async (int imageId, int listId, int containerId, ITierListService service)
            => await DeleteTierImage(imageId, listId, containerId, service)).RequireAuthorization();
    }

    private static async Task<IResult> GetImageUploadUrl(string fileName, string contentType, ITierListService service)
    {
        GetTierImageUploadUrlQuery request = new GetTierImageUploadUrlQuery { FileName = fileName, ContentType = contentType };
        TierImageResult result = await service.GetImageUploadUrlAsync(request);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Ok(result.TierImageData);
    }

    private static async Task<IResult> SaveTierImage(SaveTierImageCommand request, ITierListService service)
    {
        TierImageResult result = await service.SaveImageTierImageAsync(request);
        if (!result.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
        }

        return TypedResults.Created($"/images/{result.TierImageData?.StorageKey}", result.TierImageData);
    }

    private static async Task<IResult> UpdateTierImageNote(int imageId, UpdateTierImageNoteCommand request, ITierListService service)
    {
        if (imageId != request.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        TierImageResult commandResult = await service.UpdateTierImageAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierImageData);
    }

    private static async Task<IResult> UpdateTierImageUrl(int imageId, UpdateTierImageUrlCommand request, ITierListService service)
    {
        if (imageId != request.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        TierImageResult commandResult = await service.UpdateTierImageAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierImageData);
    }

    private static async Task<IResult> ReorderTierImage(int imageId, ReorderTierImageCommand request, ITierListService service)
    {
        if (imageId != request.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        TierImageResult commandResult = await service.ReorderTierImageAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierImageData);
    }

    private static async Task<IResult> MoveTierImage(int imageId, MoveTierImageCommand request, ITierListService service)
    {
        if (imageId != request.Id)
        {
            return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
        }

        TierImageResult commandResult = await service.MoveTierImageAsync(request);
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok(commandResult.TierImageData);
    }

    private static async Task<IResult> DeleteTierImage(int imageId, int listId, int containerId, ITierListService service)
    {
        TierImageResult commandResult = await service.DeleteTierImageAsync(new DeleteTierImageCommand { Id = imageId, ListId = listId, ContainerId = containerId });
        if (!commandResult.IsSuccess)
        {
            return ErrorHandler.HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
        }

        return TypedResults.Ok();
    }
}
