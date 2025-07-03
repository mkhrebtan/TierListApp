namespace TierList.Application.Commands.TierImage;

public record DeleteTierImageCommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }
}
