namespace TierList.Application.Queries;

public record GetTierListsQuery
{
    required public int UserId { get; init; }
}
