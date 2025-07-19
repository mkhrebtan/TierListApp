using TierList.Domain.Abstraction;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a base class for entities that contain images in a tier list.
/// </summary>
public abstract class TierImageContainer : Entity
{
    protected readonly HashSet<TierImageEntity> _images = new();

    protected TierImageContainer(int tierListId)
        : base()
    {
        TierListId = tierListId;
    }

    /// <summary>
    /// Gets the unique identifier of the tier list related to this container.
    /// </summary>
    public int TierListId { get; private init; }

    /// <summary>
    /// Gets the collection of images associated with the container.
    /// </summary>
    public IReadOnlyCollection<TierImageEntity> Images => _images;

    internal Result<TierImageEntity> ReorderImage(int imageId, Order newOrder)
    {
        TierImageEntity? imageEntity = _images.FirstOrDefault(i => i.Id == imageId);
        if (imageEntity is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.NotFound", $"Image with ID {imageId} does not exist in this container."));
        }

        int imagesCount = _images.Count;
        if (newOrder > imagesCount)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.InvalidOrder", $"New order {newOrder.Value} exceeds the number of images in this container."));
        }
        else if (newOrder == imageEntity.Order)
        {
            return Result<TierImageEntity>.Success(imageEntity);
        }

        var orderedImages = _images.OrderBy(i => i.Order.Value).ToList();
        orderedImages.Remove(imageEntity);
        orderedImages.Insert(newOrder.Value - 1, imageEntity);
        for (int i = 0; i < imagesCount; i++)
        {
            orderedImages[i].UpdateOrder(Order.Create(i + 1).Value);
        }

        return Result<TierImageEntity>.Success(imageEntity);
    }

    internal Result<TierImageEntity> MoveImageToContainer(int imageId, TierImageContainer targetContainer, Order newOrder)
    {
        if (newOrder > targetContainer._images.Count + 1)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.InvalidOrder", $"New order {newOrder.Value} exceeds the number of images in the target container."));
        }

        TierImageEntity? imageEntity = _images.FirstOrDefault(i => i.Id == imageId);
        if (imageEntity is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.NotFound", $"Image with ID {imageId} does not exist in this container."));
        }

        var orderedImages = _images.OrderBy(i => i.Order.Value).ToList();
        orderedImages.Remove(imageEntity);
        for (int i = 0; i < orderedImages.Count; i++)
        {
            orderedImages[i].UpdateOrder(Order.Create(i + 1).Value);
        }

        targetContainer.InsertImageAfterMove(imageEntity, newOrder);
        _images.Remove(imageEntity);
        return Result<TierImageEntity>.Success(imageEntity);
    }

    internal Result<TierImageEntity> RemoveImage(int imageId)
    {
        TierImageEntity? imageEntity = _images.FirstOrDefault(i => i.Id == imageId);
        if (imageEntity is null)
        {
            return Result<TierImageEntity>.Failure(new Error("TierImage.NotFound", $"Image with ID {imageId} does not exist in this container."));
        }

        _images.Remove(imageEntity);
        var orderedImages = _images.OrderBy(i => i.Order.Value).ToList();
        for (int i = 0; i < orderedImages.Count; i++)
        {
            orderedImages[i].UpdateOrder(Order.Create(i + 1).Value);
        }

        return Result<TierImageEntity>.Success(imageEntity);
    }

    private void InsertImageAfterMove(TierImageEntity imageEntity, Order order)
    {
        var orderedImages = _images.OrderBy(i => i.Order.Value).ToList();
        orderedImages.Insert(order.Value - 1, imageEntity);
        for (int i = 0; i < orderedImages.Count; i++)
        {
            orderedImages[i].UpdateOrder(Order.Create(i + 1).Value);
        }

        _images.Add(imageEntity);
        imageEntity.UpdateContainer(Id);
    }
}
