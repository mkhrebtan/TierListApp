using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierImage.Reorder;

public sealed record ReorderTierImageCommand : ICommand<TierImageDto>
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }

    required public int Order { get; init; }
}