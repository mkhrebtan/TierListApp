using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.DTOs.TierList;

/// <summary>
/// Represents a brief summary of a tier list, including its identifier, title, and timestamps.
/// </summary>
/// <remarks>This DTO is intended for scenarios where only a high-level overview of a tier list is required, such
/// as displaying a list of tier lists without their full details.</remarks>
public class TierListBriefDto : ITierListDto
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    required public string Title { get; init; }

    /// <summary>
    /// Gets the date and time when the object was created.
    /// </summary>
    public DateTime Created { get; init; }

    /// <summary>
    /// Gets the date and time when the object was last modified.
    /// </summary>
    public DateTime LastModified { get; init; }
}
