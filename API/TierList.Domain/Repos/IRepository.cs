using TierList.Domain.Abstraction;

namespace TierList.Domain.Repos;

/// <summary>
/// Defines a generic repository interface for CRUD operations on entities.
/// </summary>
/// <typeparam name="TEntity">Domain entity type.</typeparam>
public interface IRepository<TEntity>
    where TEntity : Entity
{
    /// <summary>
    /// Asynchronously retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to retrieve. Must be a positive integer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the entity of type <typeparamref
    /// name="TEntity"/>  if found; otherwise, <see langword="null"/>.</returns>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// Inserts the specified entity into the data store.
    /// </summary>
    /// <param name="entity">The entity to insert. Cannot be <see langword="null"/>.</param>
    void Insert(TEntity entity);

    /// <summary>
    /// Updates the specified entity in the data store.
    /// </summary>
    /// <remarks>This method updates the provided entity in the underlying data store.  Ensure that the entity
    /// exists in the data store before calling this method,  as behavior for non-existent entities may vary depending
    /// on the implementation.</remarks>
    /// <param name="entity">The entity to update. Must not be <see langword="null"/>.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Deletes the specified entity from the data store.
    /// </summary>
    /// <param name="entity">The entity to delete. Must not be <see langword="null"/>.</param>
    void Delete(TEntity entity);
}
