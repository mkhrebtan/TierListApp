using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a backup row entity in a tier list.
/// </summary>
public class TierBackupRowEntity : TierImageContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TierBackupRowEntity"/> class with the specified tier list
    /// identifier.
    /// </summary>
    /// <param name="tierListId">The identifier of the tier list associated with this backup row entity.</param>
    internal TierBackupRowEntity(int tierListId)
        : base(tierListId)
    {
    }

    internal Result<TierImageEntity> AddImage(Guid storageKey, string url)
    {
        Order order = Order.Create(_images.Count + 1).Value;
        Result<TierImageEntity> imageEntityResult = TierImageEntity.Create(storageKey, url, Id, order);
        if (!imageEntityResult.IsSuccess)
        {
            return imageEntityResult;
        }

        TierImageEntity imageEntity = imageEntityResult.Value;
        _images.Add(imageEntity);

        return Result<TierImageEntity>.Success(imageEntity);
    }
}
