using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a base class for entities that contain images in a tier list.
/// </summary>
public abstract class TierImageContainer : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the tier list related to this container.
    /// </summary>
    public int TierListId { get; set; }

    /// <summary>
    /// Gets or sets the tier list entity associated with the current context.
    /// </summary>
    public TierListEntity? TierList { get; set; }

    /// <summary>
    /// Gets or sets the collection of images associated with the container.
    /// </summary>
    public List<TierImageEntity> Images { get; set; } = new List<TierImageEntity>();
}
