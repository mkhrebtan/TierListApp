using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierRow.UpdateRank;

public sealed record UpdateTierRowRankCommand : ICommand<TierRowBriefDto>
{
    required public int Id { get; set; }

    required public int ListId { get; set; }

    required public string Rank { get; set; }
}