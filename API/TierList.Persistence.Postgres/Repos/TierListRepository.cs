using Microsoft.EntityFrameworkCore;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Persistence.Postgres.Repos;

/// <summary>
/// Provides repository methods for managing tier lists, rows, and images in the database.
/// </summary>
/// <remarks>This repository extends <see cref="GenericRepository{TEntity}"/> to provide specialized operations
/// for tier lists, including retrieving, adding, updating, and deleting tier rows and images. It also supports both
/// asynchronous and queryable operations for flexible data access.</remarks>
public class TierListRepository : GenericRepository<TierListEntity>, ITierListRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TierListRepository"/> class with the specified database context.
    /// </summary>
    /// <param name="context">The <see cref="TierListDbContext"/> used to access the database. Cannot be null.</param>
    public TierListRepository(TierListDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Retrieves all tier list entities from the data source.
    /// </summary>
    /// <remarks>The returned entities are retrieved with no tracking enabled, meaning they are not tracked
    /// by the context and will not reflect changes made to the database after retrieval.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"> of
    /// <see cref="TierListEntity"> objects representing all tier lists.</returns>
    public async Task<IEnumerable<TierListEntity>> GetAllAsync()
    {
        return await _context.TierLists
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a queryable collection of all <see cref="TierListEntity"/> objects from the data source.
    /// </summary>
    /// <remarks>The returned queryable collection is not tracked by the context, meaning changes to the
    /// entities will not be automatically persisted to the database. This is useful for read-only operations where
    /// tracking is unnecessary.</remarks>
    /// <returns>An <see cref="IQueryable{T}"/> representing all <see cref="TierListEntity"/> objects in the data source.</returns>
    public IQueryable<TierListEntity> GetAllQueryable()
    {
        return _context.TierLists
            .AsNoTracking();
    }

    /// <summary>
    /// Adds a new row entity to the TierImageContainers collection.
    /// </summary>
    /// <remarks>This method adds the specified <see cref="TierRowEntity"/> to the underlying data context.
    /// Ensure that the provided entity is properly initialized before calling this method.</remarks>
    /// <param name="rowEntity">The row entity to add. This parameter cannot be null.</param>
    public void AddRow(TierRowEntity rowEntity)
    {
        _context.TierImageContainers.Add(rowEntity);
    }

    /// <summary>
    /// Deletes the specified row entity from the data context.
    /// </summary>
    /// <remarks>This method removes the specified entity from the underlying data context.  Ensure that the
    /// entity exists in the context before calling this method to avoid unexpected behavior.</remarks>
    /// <param name="rowEntity">The row entity to be deleted. This parameter cannot be <see langword="null"/>.</param>
    public void DeleteRow(TierRowEntity rowEntity)
    {
        _context.TierImageContainers.Remove(rowEntity);
    }

    /// <summary>
    /// Updates the specified row entity in the database.
    /// </summary>
    /// <remarks>This method updates the provided <see cref="TierRowEntity"/> in the database context. Ensure
    /// that the entity being updated is already tracked by the context.</remarks>
    /// <param name="rowEntity">The row entity to update. This parameter cannot be <see langword="null"/>.</param>
    public void UpdateRow(TierRowEntity rowEntity)
    {
        _context.TierImageContainers.Update(rowEntity);
    }

    /// <summary>
    /// Asynchronously retrieves all tier row entities associated with the specified tier list ID.
    /// </summary>
    /// <remarks>The returned entities include their associated images and are retrieved with no tracking
    /// enabled, ensuring they are not cached in the current database context.</remarks>
    /// <param name="listId">The unique identifier of the tier list whose rows are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of
    /// <see cref="TierRowEntity"/> objects, each representing a row in the specified tier list. The collection will be
    /// empty if no rows are found.</returns>
    public async Task<IEnumerable<TierRowEntity>> GetRowsAsync(int listId)
    {
        return await _context.TierImageContainers
            .OfType<TierRowEntity>()
            .Where(r => r.TierListId == listId)
            .Include(r => r.Images)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a queryable collection of tier row entities associated with the specified tier list ID.
    /// </summary>
    /// <param name="listId">The unique identifier of the tier list whose rows are to be retrieved.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of <see cref="TierRowEntity"/> objects representing the rows in the specified
    /// tier list. The query includes related images and is configured for no tracking.</returns>
    public IQueryable<TierRowEntity> GetRowsQueryable(int listId)
    {
        return _context.TierImageContainers
            .OfType<TierRowEntity>()
            .Where(r => r.TierListId == listId)
            .Include(r => r.Images)
            .AsNoTracking();
    }

    /// <summary>
    /// Retrieves a tier row entity by its unique identifier and associated tier list identifier.
    /// </summary>
    /// <remarks>The returned entity is not tracked by the context. Use this method when read-only access to
    /// the row is sufficient.</remarks>
    /// <param name="listId">The identifier of the tier list to which the row belongs.</param>
    /// <param name="rowId">The unique identifier of the row to retrieve.</param>
    /// <returns>A <see cref="TierRowEntity"/> representing the row if found; otherwise, <see langword="null"/>.</returns>
    public async Task<TierRowEntity?> GetRowByIdAsync(int listId, int rowId)
    {
        return await _context.TierImageContainers
            .OfType<TierRowEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == rowId && r.TierListId == listId);
    }

    /// <summary>
    /// Retrieves a backup row entity associated with the specified tier list ID.
    /// </summary>
    /// <remarks>The method performs a query on the database to retrieve the first backup row entity  that
    /// matches the specified tier list ID. The query is executed with no tracking,  meaning the returned entity is not
    /// tracked by the database context.</remarks>
    /// <param name="listId">The unique identifier of the tier list to retrieve the backup row for.</param>
    /// <returns>A <see cref="TierBackupRowEntity"/> object representing the backup row if found;  otherwise, <see
    /// langword="null"/>.</returns>
    public async Task<TierBackupRowEntity?> GetBackupRowAsync(int listId)
    {
        return await _context.TierImageContainers
             .OfType<TierBackupRowEntity>()
             .AsNoTracking()
             .FirstOrDefaultAsync(r => r.TierListId == listId);
    }

    /// <summary>
    /// Asynchronously retrieves a collection of images associated with the specified list and row identifiers.
    /// </summary>
    /// <remarks>The method performs a query on the database to retrieve images associated with the specified
    /// tier list and row. The results are returned as a read-only collection and are not tracked by the Entity
    /// Framework change tracker.</remarks>
    /// <param name="listId">The identifier of the tier list to filter the images by.</param>
    /// <param name="rowId">The identifier of the row within the tier list to filter the images by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of
    /// <see cref="TierImageEntity"/> objects representing the images associated with the specified list and row.</returns>
    public async Task<IEnumerable<TierImageEntity>> GetImagesAsync(int listId, int rowId)
    {
        return await _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a queryable collection of images associated with a specific list and row.
    /// </summary>
    /// <param name="listId">The identifier of the list to which the images belong.</param>
    /// <param name="rowId">The identifier of the row within the list to which the images belong.</param>
    /// <returns>An <see cref="IQueryable{T}"/> of <see cref="TierImageEntity"/> representing the images associated with the
    /// specified list and row. The query is executed without tracking changes.</returns>
    public IQueryable<TierImageEntity> GetImagesQueryable(int listId, int rowId)
    {
        return _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking();
    }

    /// <summary>
    /// Retrieves a specific image entity by its unique identifier within a tier list and row.
    /// </summary>
    /// <remarks>This method performs a query to locate the image within the specified tier list and row. The
    /// query is executed with no tracking to optimize read-only operations.</remarks>
    /// <param name="listId">The identifier of the tier list containing the image.</param>
    /// <param name="rowId">The identifier of the row within the tier list that contains the image.</param>
    /// <param name="imageId">The unique identifier of the image to retrieve.</param>
    /// <returns>A <see cref="TierImageEntity"/> representing the image if found; otherwise, <see langword="null"/>.</returns>
    public async Task<TierImageEntity?> GetImageByIdAsync(int listId, int rowId, int imageId)
    {
        return await _context.TierImageContainers
            .Where(r => r.TierListId == listId && r.Id == rowId)
            .SelectMany(r => r.Images)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == imageId);
    }

    /// <summary>
    /// Adds a new image entity to the data context.
    /// </summary>
    /// <param name="imageEntity">The image entity to add. Cannot be null.</param>
    public void AddImage(TierImageEntity imageEntity)
    {
        _context.TierImages.Add(imageEntity);
    }

    /// <summary>
    /// Deletes the specified image entity from the data context.
    /// </summary>
    /// <remarks>This method removes the provided image entity from the underlying data context.  Ensure that
    /// the entity exists in the context before calling this method to avoid unexpected behavior.</remarks>
    /// <param name="imageEntity">The image entity to be deleted. This parameter cannot be <see langword="null"/>.</param>
    public void DeleteImage(TierImageEntity imageEntity)
    {
        _context.TierImages.Remove(imageEntity);
    }

    /// <summary>
    /// Updates the specified image entity in the database.
    /// </summary>
    /// <remarks>This method updates the provided <see cref="TierImageEntity"/> in the database context.
    /// Ensure that the entity being updated is already tracked or properly configured in the context.</remarks>
    /// <param name="imageEntity">The image entity to update. This parameter cannot be <see langword="null"/>.</param>
    public void UpdateImage(TierImageEntity imageEntity)
    {
        _context.TierImages.Update(imageEntity);
    }
}
