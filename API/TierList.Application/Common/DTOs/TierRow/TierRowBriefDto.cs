namespace TierList.Application.Common.DTOs.TierRow;

public class TierRowBriefDto
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
}
