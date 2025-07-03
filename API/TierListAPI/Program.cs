using Scalar.AspNetCore;
using TierList.Application;
using TierList.Application.Commands.TierImage;
using TierList.Application.Commands.TierList;
using TierList.Application.Commands.TierRow;
using TierList.Application.Common.DTOs;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Queries;
using TierList.Infrastructure;
using TierList.Persistence.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Tier List API";
        options.HideClientButton = true;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowLocalhost");

app.MapGet("/lists", async (ITierListService service) =>
{
    GetTierListsQuery query = new();
    IReadOnlyCollection<TierListBriefDto> result = await service.GetTierListsAsync(query);
    return TypedResults.Ok(result);
});

app.MapGet("/lists/{listId}", async (int listId, ITierListService service) =>
{
    GetTierListDataQuery query = new GetTierListDataQuery { Id = listId };
    TierListResult result = await service.GetTierListDataAsync(query);
    if (!result.IsSuccess)
    {
        return HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
    }

    return TypedResults.Ok(result.TierListData);
});

app.MapPost("/lists", async (CreateTierListCommand request, ITierListService service) =>
{
    TierListResult commandResult = await service.CreateTierListAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return Results.Created($"/lists/{commandResult.TierListData?.Id}", commandResult.TierListData);
});

app.MapPut("lists/{listId}", async (int listId, UpdateTierListCommand request, ITierListService service) =>
{
    if (listId != request.Id)
    {
        return TypedResults.BadRequest("List ID in the URL does not match the ID in the request body.");
    }

    TierListResult commandResult = await service.UpdateTierListAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierListData);
});

app.MapDelete("/lists/{listId}", async (int listId, ITierListService service) =>
{
    TierListResult commandResult = await service.DeleteTierListAsync(new DeleteTierListCommand { Id = listId });
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.NoContent();
});

app.MapPost("/rows", async (CreateTierRowCommand request, ITierListService service) =>
{
    TierRowResult commandResult = await service.CreateTierRowAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return Results.Created($"/lists/{commandResult.TierRowData?.Id}", commandResult.TierRowData);
});

app.MapPut("/rows/{rowId}/rank", async (int rowId, UpdateTierRowRankCommand request, ITierListService service) => 
{
    if (rowId != request.Id)
    {
        return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
    }

    TierRowResult commandResult = await service.UpdateTierRowAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierRowData);
});

app.MapPut("/rows/{rowId}/color", async (int rowId, UpdateTierRowColorCommand request, ITierListService service) =>
{
    if (rowId != request.Id)
    {
        return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
    }

    TierRowResult commandResult = await service.UpdateTierRowAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierRowData);
});

app.MapPut("/rows/{rowId}/order", async (int rowId, UpdateTierRowOrderCommand request, ITierListService service) =>
{
    if (rowId != request.Id)
    {
        return TypedResults.BadRequest("Row ID in the URL does not match the ID in the request body.");
    }

    TierRowResult commandResult = await service.UpdateTierRowAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierRowData);
});

app.MapDelete("/rows/{rowId}", async (int rowId, int listId, bool isDeleteWithImages, ITierListService service) =>
{
    TierRowResult commandResult = await service.DeleteTierRowAsync(new DeleteTierRowCommand { Id = rowId, ListId = listId, IsDeleteWithImages = isDeleteWithImages });
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok();
});

app.MapGet("/images/upload-url", async (string fileName, string contentType, ITierListService service) =>
{
    GetTierImageUploadUrlQuery request = new GetTierImageUploadUrlQuery { FileName = fileName, ContentType = contentType };
    TierImageResult result = await service.GetImageUploadUrlAsync(request);
    if (!result.IsSuccess)
    {
        return HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
    }

    return TypedResults.Ok(result.TierImageData);
});

app.MapPost("/images", async (SaveTierImageCommand request, ITierListService service) =>
{
    TierImageResult result = await service.SaveImageTierImageAsync(request);
    if (!result.IsSuccess)
    {
        return HandleError((ErrorType)result.ErrorType!, result.ErrorMessage!);
    }

    return TypedResults.Created($"/images/{result.TierImageData?.StorageKey}", result.TierImageData);
});

app.MapPut("/images/{imageId}/note", async (int imageId, UpdateTierImageNoteCommand request, ITierListService service) =>
{
    if (imageId != request.Id)
    {
        return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
    }

    TierImageResult commandResult = await service.UpdateTierImageAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierImageData);
});

app.MapPut("/images/{imageId}/url", async (int imageId, UpdateTierImageUrlCommand request, ITierListService service) =>
{
    if (imageId != request.Id)
    {
        return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
    }

    TierImageResult commandResult = await service.UpdateTierImageAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierImageData);
});

app.MapPut("/images/{imageId}/move", async (int imageId, MoveTierImageCommand request, ITierListService service) =>
{
    if (imageId != request.Id)
    {
        return TypedResults.BadRequest("Image ID in the URL does not match the ID in the request body.");
    }

    TierImageResult commandResult = await service.MoveTierImageAsync(request);
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok(commandResult.TierImageData);
});

app.MapDelete("/images/{imageId}", async (int imageId, int listId, int containerId, ITierListService service) =>
{
    TierImageResult commandResult = await service.DeleteTierImageAsync(new DeleteTierImageCommand { Id = imageId, ListId = listId, ContainerId = containerId });
    if (!commandResult.IsSuccess)
    {
        return HandleError((ErrorType)commandResult.ErrorType!, commandResult.ErrorMessage!);
    }

    return TypedResults.Ok();
});

await app.RunAsync();

static IResult HandleError(ErrorType error, string errorMessage)
{
    IResult errorResult = error switch
    {
        ErrorType.NotFound => TypedResults.NotFound(errorMessage),
        ErrorType.ValidationError => TypedResults.BadRequest(errorMessage),
        ErrorType.SaveDataError => TypedResults.InternalServerError(errorMessage),
        _ => TypedResults.InternalServerError("An unexpected error occurred while retrieving the upload URL.")
    };
    return errorResult;
}