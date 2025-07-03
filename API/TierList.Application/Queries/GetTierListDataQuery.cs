namespace TierList.Application.Queries;

public record GetTierListDataQuery
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    required public int Id { get; init; }
}
