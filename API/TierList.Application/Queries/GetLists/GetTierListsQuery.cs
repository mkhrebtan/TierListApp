using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Queries.GetLists;

public sealed record GetTierListsQuery : IQuery<IReadOnlyCollection<TierListBriefDto>>
{
    required public int UserId { get; init; }
}