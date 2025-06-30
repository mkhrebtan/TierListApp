namespace TierList.Application.Common.DTOs;

/// <summary>
/// Represents a data transfer object for a tier row, containing information such as rank, color, order, and associated
/// images.
/// </summary>
/// <remarks>This class is typically used to encapsulate tier-related data for transfer between application layers
/// or services. It includes properties for identifying the tier, its rank, display color, order, and associated
/// images.</remarks>
public class TierRowDto
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the rank associated with the entity.
    /// </summary>
    required public string Rank { get; init; }

    /// <summary>
    /// Gets the hexadecimal color code representing the color.
    /// </summary>
    required public string ColorHex { get; init; }

    /// <summary>
    /// Gets the order value associated with this instance.
    /// </summary>
    public int Order { get; init; }

    /// <summary>
    /// Gets the collection of images associated with the tier.
    /// </summary>
    public IReadOnlyCollection<TierImageDto> Images { get; init; } = new List<TierImageDto>();
}
