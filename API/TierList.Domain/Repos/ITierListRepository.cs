using TierList.Domain.Abstraction;
using TierList.Domain.Entities;

namespace TierList.Domain.Repos;

/// <summary>
/// Defines a repository interface for managing tier lists and their associated entities,  including rows and images,
/// with support for both asynchronous and queryable operations.
/// </summary>
/// <remarks>This interface provides methods for retrieving, adding, updating, and deleting tier list entities,
/// rows, and images. It supports both asynchronous operations for data retrieval and manipulation,  as well as
/// queryable operations for advanced querying scenarios.  Implementations of this interface are expected to handle
/// persistence and data access logic.</remarks>
public interface ITierListRepository : IRepository<TierListEntity>
{
    /// <summary>
    /// Asynchronously retrieves all tier list entities.
    /// </summary>
    /// <remarks>This method retrieves all tier list entities without applying any filters or pagination.  The
    /// caller is responsible for handling the returned collection appropriately.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of
    /// <see cref="TierListEntity"/> objects representing all tier list entities.</returns>
    Task<IEnumerable<TierListEntity>> GetAllAsync();

    /// <summary>
    /// Retrieves an <see cref="IQueryable{T}"/> of all <see cref="TierListEntity"/> objects in the data source.
    /// </summary>
    /// <remarks>This method is intended for scenarios where deferred execution or query composition is
    /// required. The returned <see cref="IQueryable{T}"/> allows further filtering, sorting, and projection before
    /// execution against the underlying data source.</remarks>
    /// <returns>An <see cref="IQueryable{T}"/> representing all <see cref="TierListEntity"/> objects.</returns>
    IQueryable<TierListEntity> GetAllQueryable();

    /// <summary>
    /// Adds a new row to the tier collection.
    /// </summary>
    /// <remarks>This method appends the specified <paramref name="rowEntity"/> to the collection. Ensure that
    /// the provided entity meets any required validation criteria before calling this method.</remarks>
    /// <param name="rowEntity">The row entity to add. This parameter cannot be <see langword="null"/>.</param>
    void AddRow(TierRowEntity rowEntity);

    /// <summary>
    /// Deletes the specified row entity from the data store.
    /// </summary>
    /// <remarks>This method removes the provided row entity from the underlying data store.  Ensure that the
    /// entity exists in the data store before calling this method to avoid unexpected behavior.</remarks>
    /// <param name="rowEntity">The row entity to delete. This parameter cannot be <see langword="null"/>.</param>
    void DeleteRow(TierRowEntity rowEntity);

    /// <summary>
    /// Updates the specified row in the data store with the provided entity values.
    /// </summary>
    /// <remarks>This method updates an existing row in the data store.  Ensure that the provided <paramref
    /// name="rowEntity"/> contains valid data  and corresponds to an existing row.</remarks>
    /// <param name="rowEntity">The entity containing the updated values for the row.  Must not be <see langword="null"/>.</param>
    void UpdateRow(TierRowEntity rowEntity);

    /// <summary>
    /// Asynchronously retrieves a collection of rows associated with the specified list identifier.
    /// </summary>
    /// <param name="listId">The unique identifier of the list whose rows are to be retrieved. Must be a positive integer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of
    /// <see cref="TierRowEntity"/> objects representing the rows in the specified list. If no rows are found, the
    /// result will be an empty collection.</returns>
    Task<IEnumerable<TierRowEntity>> GetRowsAsync(int listId);

    /// <summary>
    /// Retrieves a queryable collection of rows associated with the specified list ID.
    /// </summary>
    /// <param name="listId">The unique identifier of the list whose rows are to be retrieved.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of <see cref="TierRowEntity"/> representing the rows in the specified list. The
    /// result can be further filtered, sorted, or projected using LINQ queries.</returns>
    IQueryable<TierRowEntity> GetRowsQueryable(int listId);

    /// <summary>
    /// Asynchronously retrieves a row entity by its unique identifiers.
    /// </summary>
    /// <remarks>Use this method to retrieve a specific row entity from a list by providing its unique
    /// identifiers.  Ensure that the identifiers are valid and correspond to an existing row.</remarks>
    /// <param name="listId">The identifier of the list containing the row.</param>
    /// <param name="rowId">The identifier of the row to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the  <see cref="TierRowEntity"/> if
    /// found; otherwise, <see langword="null"/>.</returns>
    Task<TierRowEntity?> GetRowByIdAsync(int listId, int rowId);

    /// <summary>
    /// Retrieves a backup row entity for the specified list ID.
    /// </summary>
    /// <param name="listId">The unique identifier of the list for which the backup row is to be retrieved. Must be a non-negative integer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the backup row entity associated
    /// with the specified list ID,  or <see langword="null"/> if no matching entity is found.</returns>
    Task<TierBackupRowEntity?> GetBackupRowAsync(int listId);

    /// <summary>
    /// Asynchronously retrieves a collection of images associated with the specified list and row identifiers.
    /// </summary>
    /// <remarks>Use this method to retrieve images tied to a specific list and row. Ensure that the provided
    /// identifiers are valid and correspond to existing data.</remarks>
    /// <param name="listId">The unique identifier of the list containing the images.</param>
    /// <param name="rowId">The unique identifier of the row within the list for which images are retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of
    /// <see cref="TierImageEntity"/> objects representing the images associated with the specified list and row.</returns>
    Task<IEnumerable<TierImageEntity>> GetImagesAsync(int listId, int rowId);

    /// <summary>
    /// Retrieves a queryable collection of tier images associated with the specified list and row identifiers.
    /// </summary>
    /// <remarks>This method does not execute the query immediately. The returned <see cref="IQueryable{T}"/>
    /// allows deferred execution, enabling additional filtering, sorting, or projection before the query is
    /// executed.</remarks>
    /// <param name="listId">The identifier of the list to which the images belong. Must be a valid, non-negative integer.</param>
    /// <param name="rowId">The identifier of the row within the list to which the images belong. Must be a valid, non-negative integer.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of <see cref="TierImageEntity"/> representing the images associated with the
    /// specified list and row. The result can be further filtered or queried using LINQ.</returns>
    IQueryable<TierImageEntity> GetImagesQueryable(int listId, int rowId);

    /// <summary>
    /// Retrieves an image entity by its unique identifiers within a specific list and row.
    /// </summary>
    /// <remarks>Use this method to retrieve a specific image entity when you know its list, row, and image
    /// identifiers.</remarks>
    /// <param name="listId">The identifier of the list containing the image.</param>
    /// <param name="rowId">The identifier of the row within the list where the image is located.</param>
    /// <param name="imageId">The unique identifier of the image to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the  <see cref="TierImageEntity"/>
    /// if found; otherwise, <see langword="null"/>.</returns>
    Task<TierImageEntity?> GetImageByIdAsync(int listId, int rowId, int imageId);

    /// <summary>
    /// Adds a new image to the collection.
    /// </summary>
    /// <remarks>The image entity must contain valid data. Ensure that all required properties of the
    /// <paramref name="imageEntity"/> are properly initialized before calling this method.</remarks>
    /// <param name="imageEntity">The image entity to add. This parameter cannot be null.</param>
    void AddImage(TierImageEntity imageEntity);

    /// <summary>
    /// Deletes the specified image entity from the system.
    /// </summary>
    /// <remarks>This method removes the provided image entity from the system. Ensure that the  <paramref
    /// name="imageEntity"/> represents a valid and existing image before calling this method.</remarks>
    /// <param name="imageEntity">The image entity to delete. This parameter cannot be <see langword="null"/>.</param>
    void DeleteImage(TierImageEntity imageEntity);

    /// <summary>
    /// Updates the specified image entity in the system.
    /// </summary>
    /// <remarks>Use this method to update the properties of an existing image entity.  Ensure that the
    /// provided <paramref name="imageEntity"/> contains valid data  and corresponds to an existing record in the
    /// system.</remarks>
    /// <param name="imageEntity">The image entity to update. This parameter cannot be <see langword="null"/>.</param>
    void UpdateImage(TierImageEntity imageEntity);
}
