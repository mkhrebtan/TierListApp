using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a tier row entity with a rank, color, and order, used in tier-based categorization systems.
/// </summary>
/// <remarks>This class extends <see cref="TierImageContainer"/> and provides additional properties to define the
/// rank, color, and order of a tier row. The <see cref="Rank"/> and <see cref="ColorHex"/> properties are required and
/// must be initialized.</remarks>
public class TierRowEntity : TierImageContainer
{
    /// <summary>
    /// Gets or sets the rank of the tier row.
    /// </summary>
    required public string Rank { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hexadecimal color code representing the color.
    /// </summary>
    required public string ColorHex { get; set; } = "#FFFFFF";

    /// <summary>
    /// Gets or sets the order in which this item should be processed or displayed.
    /// </summary>
    public int Order { get; set; }
}
