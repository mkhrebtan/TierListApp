using Microsoft.EntityFrameworkCore;
using TierList.Domain.Abstraction;
using TierList.Domain.Repos;

namespace TierList.Persistence.Postgres.Repos;

/// <summary>
/// Provides a generic implementation of the <see cref="IRepository{TEntity}"/> interface for managing entities in a
/// database context.
/// </summary>
/// <remarks>This repository provides basic CRUD (Create, Read, Update, Delete) operations for entities of type
/// <typeparamref name="TEntity"/>. It uses Entity Framework Core's <see cref="DbSet{TEntity}"/> to interact with the
/// database.</remarks>
/// <typeparam name="TEntity">The type of the entity managed by the repository. Must be a class that implements the <see cref="IEntity"/>
/// interface.</typeparam>
public class GenericRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Represents the database context used for accessing and managing data in the tier list application.
    /// </summary>
    /// <remarks>This field is intended for use within derived classes to interact with the underlying
    /// database. It provides access to the database context for performing CRUD operations and querying data.</remarks>
    protected readonly TierListDbContext _context;

    /// <summary>
    /// Represents the database set for the specified entity type.
    /// </summary>
    /// <remarks>This field provides access to the underlying <see cref="DbSet{TEntity}"/> for performing
    /// database operations such as querying, adding, updating, or deleting entities of type <typeparamref
    /// name="TEntity"/>. It is intended to be used within derived classes to interact with the database
    /// context.</remarks>
    protected readonly DbSet<TEntity> _dbSet;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class with the specified database
    /// context.
    /// </summary>
    /// <param name="context">The <see cref="TierListDbContext"/> instance used to access the database.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <exception cref="InvalidOperationException">Thrown if the entity type <typeparamref name="TEntity"/> is not registered in the provided <see
    /// cref="TierListDbContext"/>.</exception>
    public GenericRepository(TierListDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>() ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} is not registered in the DbContext.");
    }

    /// <inheritdoc/>
    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc/>
    public void Insert(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    /// <inheritdoc/>
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc/>
    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}
