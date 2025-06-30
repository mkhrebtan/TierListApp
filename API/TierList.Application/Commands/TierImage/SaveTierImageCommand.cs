namespace TierList.Application.Commands.TierImage;

public record SaveTierImageCommand
{
    public int Id { get; init; }

    public Guid StorageKey { get; init; }

    public int Order { get; init; }

    public string Note { get; init; } = string.Empty;

    public int ContainerId { get; init; }
}
