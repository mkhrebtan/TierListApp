namespace TierList.Application.Commands.TierRow;

public record CreateTierRowCommand
{
    required public int ListId { get; init; }

    public string Rank { get; init; } = "New";

    public string ColorHex { get; init; } = "#FFFFFF";

    public int? Order { get; init; } = null;
}
