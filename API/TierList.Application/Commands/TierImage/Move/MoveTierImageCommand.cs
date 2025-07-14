using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierImage.Move;

public sealed record MoveTierImageCommand : ICommand<TierImageDto>
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int FromContainerId { get; init; }

    required public int ToContainerId { get; init; }

    required public int Order { get; init; }
}