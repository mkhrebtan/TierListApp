namespace TierList.Application.Commands.TierRow;

public record DeleteTierRowCommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public bool IsDeleteWithImages { get; init; }
}
