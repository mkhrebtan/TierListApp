namespace TierList.Application.Common.Interfaces;

/// <summary>
/// Represents a data transfer object (DTO) for a tier list, containing its unique identifier and title.
/// </summary>
public interface ITierListDto
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    int Id { get; init; }

    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    string Title { get; init; }
}
