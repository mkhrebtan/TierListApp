namespace TierList.Application.Commands.TierImage;

public record SaveTierImageCommand
{
    required public Guid StorageKey { get; init; }

    required public string Url { get; init; }

    required public int Order { get; init; }

    public string Note { get; init; } = string.Empty;

    required public int ContainerId { get; init; }
}
