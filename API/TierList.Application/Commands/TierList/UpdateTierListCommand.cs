namespace TierList.Application.Commands.TierList;

public record UpdateTierListCommand
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    required public int Id { get; init; }

    /// <summary>
    /// Gets the title associated with the object.
    /// </summary>
    public string? Title { get; init; }
}
