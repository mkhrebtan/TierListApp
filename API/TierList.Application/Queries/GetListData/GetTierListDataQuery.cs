using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Queries.GetListData;

public sealed record GetTierListDataQuery : IQuery<TierListDataDto>
{
    /// <summary>
    /// Gets the unique identifier for the entity.
    /// </summary>
    required public int Id { get; init; }

    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    required public int UserId { get; init; }
}