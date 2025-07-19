using System.Text.RegularExpressions;
using TierList.Domain.Abstraction;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a tier list entity, which includes metadata and associated containers for organizing tiered content.
/// </summary>
/// <remarks>A tier list entity is used to define a structured list with tiers, including a title, creation and
/// modification timestamps,  and a collection of associated containers that hold tiered content.</remarks>
public class TierListEntity : AggregateRoot
{
    /// <summary>
    /// Represents the maximum allowable length for a title.
    /// </summary>
    public const int MaxTitleLength = 100;

    private readonly HashSet<TierImageContainer> _containers = new();

    private TierListEntity(string title, int userId)
        : base()
    {
        Title = title;
        UserId = userId;
        Created = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the unique identifier for the user owning list entity.
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Gets the date and time when the entity was created.
    /// </summary>
    public DateTime Created { get; private set; }

    /// <summary>
    /// Gets the date and time when the object was last modified.
    /// </summary>
    public DateTime LastModified { get; private set; }

    /// <summary>
    /// Gets the collection of containers associated with the tier images.
    /// </summary>
    public IReadOnlyCollection<TierImageContainer> Containers => _containers;

    public static Result<TierListEntity> Create(string title, int userId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<TierListEntity>.Failure(new Error("TierList.EmptyTitle", "Title cannot be null or empty."));
        }

        if (title.Length > MaxTitleLength)
        {
            return Result<TierListEntity>.Failure(new Error("TierList.TitleTooLong", $"Title cannot exceed {MaxTitleLength} characters."));
        }

        return Result<TierListEntity>.Success(new TierListEntity(title, userId));
    }

    public Result InitializeDefaultContainers()
    {
        if (_containers.Count != 0)
        {
            return Result.Failure(new Error("TierList.ContainersAlreadyInitialized", "Default containers have already been initialized."));
        }

        var defaultContainers = new List<TierImageContainer>
        {
            TierRowEntity.Create(Id, "S", "#FFBF7F", Order.Create(1).Value).Value,
            TierRowEntity.Create(Id, "A", "#FFDF7F", Order.Create(2).Value).Value,
            TierRowEntity.Create(Id, "B", "#FFFF7F", Order.Create(3).Value).Value,
            new TierBackupRowEntity(Id),
        };

        foreach (var container in defaultContainers)
        {
            _containers.Add(container);
        }

        LastModified = DateTime.UtcNow;
        return Result.Success();
    }

    public Result UpdateTitle(string newTitle, int userId)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            return Result.Failure(new Error("TierList.EmptyTitle", "Title cannot be null or empty."));
        }

        if (newTitle.Length > MaxTitleLength)
        {
            return Result.Failure(new Error("TierList.TitleTooLong", $"Title cannot exceed {MaxTitleLength} characters."));
        }

        if (UserId != userId)
        {
            return Result.Failure(new Error("TierList.Unauthorized", "You are not authorized to update this tier list title."));
        }

        Title = newTitle;
        LastModified = DateTime.UtcNow;
        return Result.Success();
    }

    public Result<TierRowEntity> AddRow(string rank, string colorHex, Order order)
    {
        var existingRows = _containers.OfType<TierRowEntity>();
        int rowsCount = existingRows.Count();
        if (order > rowsCount + 1)
        {
            return Result<TierRowEntity>.Failure(new Error("TierList.InvalidOrder", "Order cannot be greater than the number of existing rows plus one."));
        }

        Result<TierRowEntity> newRowResult = TierRowEntity.Create(Id, rank, colorHex, order);
        if (!newRowResult.IsSuccess)
        {
            return newRowResult;
        }

        foreach (var existingRow in existingRows.Where(r => r.Order >= order).ToList())
        {
            existingRow.IncrementOrder();
        }

        TierRowEntity newRow = newRowResult.Value;
        _containers.Add(newRow);
        LastModified = DateTime.UtcNow;

        return Result<TierRowEntity>.Success(newRow);
    }

    public Result RemoveRow(int rowId)
    {
        var existingRows = _containers.OfType<TierRowEntity>().ToList();

        TierRowEntity? rowEntity = existingRows
            .FirstOrDefault(r => r.Id == rowId);
        if (rowEntity is null)
        {
            return Result.Failure(new Error("TierList.RowNotFound", $"Row with ID {rowId} does not exist in this tier list."));
        }

        var removedRowOrder = rowEntity.Order.Value;
        _containers.Remove(rowEntity);

        foreach (var existingRow in existingRows.Where(r => r.Order.Value > removedRowOrder))
        {
            existingRow.DecrementOrder();
        }

        LastModified = DateTime.UtcNow;
        return Result.Success();
    }

    public Result<TierRowEntity> UpdateRowColor(int rowId, string newColorHex)
    {
        TierRowEntity? rowEntity = _containers
            .OfType<TierRowEntity>()
            .FirstOrDefault(r => r.Id == rowId);
        if (rowEntity is null)
        {
            return Result<TierRowEntity>.Failure(new Error("TierList.RowNotFound", $"Row with ID {rowId} does not exist in this tier list."));
        }

        rowEntity.UpdateColor(newColorHex);

        LastModified = DateTime.UtcNow;
        return Result<TierRowEntity>.Success(rowEntity);
    }

    public Result<TierRowEntity> UpdateRowRank(int rowId, string newRank)
    {
        TierRowEntity? rowEntity = _containers
            .OfType<TierRowEntity>()
            .FirstOrDefault(r => r.Id == rowId);
        if (rowEntity is null)
        {
            return Result<TierRowEntity>.Failure(new Error("TierList.RowNotFound", $"Row with ID {rowId} does not exist in this tier list."));
        }

        rowEntity.UpdateRank(newRank);

        LastModified = DateTime.UtcNow;
        return Result<TierRowEntity>.Success(rowEntity);
    }

    public Result<TierRowEntity> UpdateRowOrder(int rowId, Order newOrder)
    {
        List<TierRowEntity> rows = _containers.OfType<TierRowEntity>().ToList();
        TierRowEntity? rowEntity = rows
            .FirstOrDefault(r => r.Id == rowId);
        if (rowEntity is null)
        {
            return Result<TierRowEntity>.Failure(new Error("TierList.RowNotFound", $"Row with ID {rowId} does not exist in this tier list."));
        }
        else if (newOrder > rows.Count)
        {
            return Result<TierRowEntity>.Failure(new Error("TierList.InvalidOrder", "New order cannot be greater than the number of existing rows."));
        }
        else if (newOrder == rowEntity.Order)
        {
            return Result<TierRowEntity>.Success(rowEntity);
        }

        var orderedRows = rows.OrderBy(r => r.Order.Value).ToList();
        orderedRows.Remove(rowEntity);
        orderedRows.Insert(newOrder.Value - 1, rowEntity);
        for (int i = 0; i < orderedRows.Count; i++)
        {
            orderedRows[i].UpdateOrder(Order.Create(i + 1).Value);
        }

        LastModified = DateTime.UtcNow;
        return Result<TierRowEntity>.Success(rowEntity);
    }

    public Result<TierImageEntity> AddImage(Guid storageKey, string url)
    {
        TierBackupRowEntity? backupRow = _containers.OfType<TierBackupRowEntity>().FirstOrDefault();
        if (backupRow is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.BackupRowNotFound", "Backup row does not exist in this tier list."));
        }

        Result<TierImageEntity> imageEntityResult = backupRow.AddImage(storageKey, url);
        if (!imageEntityResult.IsSuccess)
        {
            return imageEntityResult;
        }

        LastModified = DateTime.UtcNow;
        return imageEntityResult;
    }

    public Result<TierImageEntity> ReorderImage(int imageId, Order newOrder)
    {
        TierImageContainer? container = _containers
            .FirstOrDefault(c => c.Images.Any(i => i.Id == imageId));
        if (container is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.ImageNotFound", $"Image with ID {imageId} does not exist in this tier list."));
        }

        Result<TierImageEntity> imageEntityResult = container.ReorderImage(imageId, newOrder);
        if (!imageEntityResult.IsSuccess)
        {
            return imageEntityResult;
        }

        LastModified = DateTime.UtcNow;
        return imageEntityResult;
    }

    public Result<TierImageEntity> MoveImage(int imageId, int targetContainerId, Order newOrder)
    {
        TierImageContainer? sourceContainer = _containers
            .FirstOrDefault(c => c.Images.Any(i => i.Id == imageId));
        if (sourceContainer is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.ImageNotFound", $"Image with ID {imageId} does not exist in this tier list."));
        }

        if (sourceContainer.Id == targetContainerId)
        {
            return sourceContainer.ReorderImage(imageId, newOrder);
        }

        TierImageContainer? targetContainer = _containers
            .FirstOrDefault(c => c.Id == targetContainerId);
        if (targetContainer is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.TargetContainerNotFound", $"Target container with ID {targetContainerId} does not exist in this tier list."));
        }

        Result<TierImageEntity> imageEntityResult = sourceContainer.MoveImageToContainer(imageId, targetContainer, newOrder);
        if (!imageEntityResult.IsSuccess)
        {
            return imageEntityResult;
        }

        LastModified = DateTime.UtcNow;
        return imageEntityResult;
    }

    public Result<TierImageEntity> UpdateImageNote(int imageId, string note)
    {
        TierImageEntity? imageEntity = _containers
            .SelectMany(c => c.Images)
            .FirstOrDefault(i => i.Id == imageId);
        if (imageEntity is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.ImageNotFound", $"Image with ID {imageId} does not exist in this tier list."));
        }

        Result imageEntityResult = imageEntity.UpdateNote(note);
        if (!imageEntityResult.IsSuccess)
        {
            return Result<TierImageEntity>.Failure(imageEntityResult.Error);
        }

        LastModified = DateTime.UtcNow;
        return Result<TierImageEntity>.Success(imageEntity);
    }

    public Result<TierImageEntity> UpdateImageUrl(int imageId, string url)
    {
        TierImageEntity? imageEntity = _containers
            .SelectMany(c => c.Images)
            .FirstOrDefault(i => i.Id == imageId);
        if (imageEntity is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.ImageNotFound", $"Image with ID {imageId} does not exist in this tier list."));
        }

        Result imageEntityResult = imageEntity.UpdateUrl(url);
        if (!imageEntityResult.IsSuccess)
        {
            return Result<TierImageEntity>.Failure(imageEntityResult.Error);
        }

        LastModified = DateTime.UtcNow;
        return Result<TierImageEntity>.Success(imageEntity);
    }

    public Result<TierImageEntity> RemoveImage(int imageId)
    {
        TierImageContainer? container = _containers
            .FirstOrDefault(c => c.Images.Any(i => i.Id == imageId));
        if (container is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierList.ImageNotFound", $"Image with ID {imageId} does not exist in this tier list."));
        }

        Result<TierImageEntity> imageEntityResult = container.RemoveImage(imageId);
        if (!imageEntityResult.IsSuccess)
        {
            return imageEntityResult;
        }

        LastModified = DateTime.UtcNow;
        return imageEntityResult;
    }

    public IReadOnlyCollection<TierRowEntity> GetTierRows()
    {
        return _containers.OfType<TierRowEntity>().ToList().AsReadOnly();
    }

    public TierBackupRowEntity GetTierBackupRow()
    {
        return _containers.OfType<TierBackupRowEntity>().First();
    }
}
