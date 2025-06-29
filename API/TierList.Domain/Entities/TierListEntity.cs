using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a tier list entity, which includes metadata and associated containers for organizing tiered content.
/// </summary>
/// <remarks>A tier list entity is used to define a structured list with tiers, including a title, creation and
/// modification timestamps,  and a collection of associated containers that hold tiered content.</remarks>
public class TierListEntity : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title associated with the object.
    /// </summary>
    required public string Title { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the object was last modified.
    /// </summary>
    public DateTime LastModified { get; set; }

    /// <summary>
    /// Gets or sets the collection of containers associated with the tier images.
    /// </summary>
    public List<TierImageContainer> Containers { get; set; } = new List<TierImageContainer>();
}
