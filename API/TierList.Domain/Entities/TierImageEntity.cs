using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents an image entity associated with a specific tier, including its metadata and relationship to a container.
/// </summary>
/// <remarks>This entity is typically used to store information about images, such as their storage key, display order,
/// and optional notes. It also includes a reference to the container that groups related images.</remarks>
public class TierImageEntity : IEntity
{
    /// <inheritdoc />
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier used as the storage key.
    /// </summary>
    public Guid StorageKey { get; set; }

    /// <summary>
    /// Gets or sets the order in which this item should be processed or displayed.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets a note or comment associated with the object.
    /// </summary>
    public string Note { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the container that holds image data.
    /// </summary>
    public int ContainerId { get; set; }

    /// <summary>
    /// Gets or sets the container that holds image data.
    /// </summary>
    public TierImageContainer? Container { get; set; }
}