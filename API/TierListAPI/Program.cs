using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Persistence.Postgres;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);

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

app.MapGet("/lists", (ITierListRepository repo) =>
{
    var lists = repo.GetAll();
    var listsDTOs = lists.Select(l => new ReadTierListDTO
    {
        Id = l.Id,
        Title = l.Title,
        Created = l.Created,
        LastModified = l.LastModified
    });
    return listsDTOs.ToList();
});

app.MapPost("/lists", (CreateTierListDTO list, ITierListRepository repo, IUnitOfWork uow) => 
{
    TierListEntity tierList = new()
    {
        Title = list.Title,
        Created = DateTime.UtcNow,
        LastModified = DateTime.UtcNow
    };

    try
    {
        uow.CreateTransaction();
        
        repo.Insert(tierList);

        uow.SaveChanges();
        uow.CommitTransaction();
    }
    catch (InvalidOperationException ex)
    {
        uow.RollbackTransaction();
        return Results.Problem(ex.Message, statusCode: 500);
    }
    catch (Exception)
    {
        uow.RollbackTransaction();
        return Results.Problem("An unexpected error occurred.", statusCode: 500);
    }
    return Results.Created($"/lists/{tierList.Id}", list);
});

app.MapDelete("/lists/{listId}", (int listId, ITierListRepository repo, IUnitOfWork uow) =>
{
    TierListEntity? listToDelete = repo.GetById(listId);
    if (listToDelete is null)
    {
        return Results.NotFound($"List with ID {listId} not found.");
    }

    try
    {
        uow.CreateTransaction();
        
        repo.Delete(listToDelete);
        
        uow.SaveChanges();
        uow.CommitTransaction();
    }
    catch (InvalidOperationException ex)
    {
        uow.RollbackTransaction();
        return Results.Problem(ex.Message, statusCode: 500);
    }
    catch (Exception)
    {
        uow.RollbackTransaction();
        return Results.Problem("An unexpected error occurred.", statusCode: 500);
    }
    return Results.Ok($"List with ID {listId} deleted successfully.");
});

app.Run();

class ReadTierListDTO
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastModified { get; set; } = DateTime.Now;
}

class CreateTierListDTO
{
    public string Title { get; set; } = string.Empty;
}
