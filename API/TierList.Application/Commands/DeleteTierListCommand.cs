namespace TierList.Application.Commands;

public record DeleteTierListCommand
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    public int Id { get; init; }
}
