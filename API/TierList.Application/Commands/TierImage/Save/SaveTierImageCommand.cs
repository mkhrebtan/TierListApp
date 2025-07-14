using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierImage.Save;

public sealed record SaveTierImageCommand : ICommand<TierImageDto>
{
    required public Guid StorageKey { get; init; }

    required public string Url { get; init; }

    required public int Order { get; init; }

    public string Note { get; init; } = string.Empty;

    required public int ContainerId { get; init; }

    required public int ListId { get; init; }
}