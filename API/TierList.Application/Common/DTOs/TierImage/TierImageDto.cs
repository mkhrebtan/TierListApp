namespace TierList.Application.Common.DTOs.TierImage;

/// <summary>
/// Represents an image associated with a specific tier, including metadata such as its URL, note, and order within a
/// container.
/// </summary>
/// <remarks>This data transfer object (DTO) is typically used to encapsulate information about tier images for
/// transport between application layers.</remarks>
public class TierImageDto
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public int Id { get; init; }

    public Guid StorageKey { get; init; }

    /// <summary>
    /// Gets the URL associated with the current instance.
    /// </summary>
    required public string Url { get; init; }

    required public string Note { get; init; }

    public int ContainerId { get; init; }

    public int Order { get; init; }
}
