using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierRow.Create;

public sealed record CreateTierRowCommand : ICommand<TierRowBriefDto>
{
    required public int ListId { get; init; }

    public string Rank { get; init; } = "New";

    public string ColorHex { get; init; } = "#FFFFFF";

    required public int Order { get; init; }
}