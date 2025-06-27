using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using TierList.Application;
using TierList.Application.Commands;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Queries;
using TierList.Application.Queries.DTOs;
using TierList.Persistence.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplicationServices();

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

app.MapGet("/lists", (ITierListService service) =>
{
    GetTierListsQuery query = new ();
    IReadOnlyCollection<TierListBriefDTO> result = service.GetTierLists(query);
    return TypedResults.Ok(result);
});

app.MapPost("/lists", (CreateTierListCommand request, ITierListService service) =>
{
    TierListResult commandResult = service.CreateTierList(request);
    if (!commandResult.IsSuccess)
    {
        IResult result = commandResult.ErrorType switch
        {
            ErrorType.ValidationError => TypedResults.BadRequest(commandResult.ErrorMessage),
            ErrorType.SaveDataError => Results.Problem(commandResult.ErrorMessage, statusCode: 500),
            _ => Results.Problem("An unexpected error occurred while creating the tier list.", statusCode: 500)
        };
        return result;
    }

    return Results.Created($"/lists/{commandResult.TierListData?.Id}", commandResult.TierListData);
});

app.MapDelete("/lists/{listId}", (int listId, ITierListService service) =>
{
    TierListResult commandResult = service.DeleteTierList(new DeleteTierListCommand(listId));
    if (!commandResult.IsSuccess)
    {
        IResult result = commandResult.ErrorType switch
        {
            ErrorType.NotFound => TypedResults.NotFound(commandResult.ErrorMessage),
            ErrorType.ValidationError => TypedResults.BadRequest(commandResult.ErrorMessage),
            ErrorType.SaveDataError => Results.Problem(commandResult.ErrorMessage, statusCode: 500),
            _ => Results.Problem("An unexpected error occurred while deleting the tier list.", statusCode: 500)
        };
        return result;
    }

    return TypedResults.NoContent();
});

app.MapPut("lists/{listId}", (int listId, UpdateTierListCommand request, ITierListService service) =>
{
    if (listId != request.Id)
    {
        return TypedResults.BadRequest("List ID in the URL does not match the ID in the request body.");
    }

    TierListResult commandResult = service.UpdateTierList(request);
    if (!commandResult.IsSuccess)
    {
        IResult result = commandResult.ErrorType switch
        {
            ErrorType.NotFound => TypedResults.NotFound(commandResult.ErrorMessage),
            ErrorType.ValidationError => TypedResults.BadRequest(commandResult.ErrorMessage),
            ErrorType.SaveDataError => Results.Problem(commandResult.ErrorMessage, statusCode: 500),
            _ => Results.Problem("An unexpected error occurred while updating the tier list.", statusCode: 500)
        };
        return result;
    }

    return TypedResults.Ok(commandResult.TierListData);
});

await app.RunAsync();
