using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierImage.Delete;

public sealed record DeleteTierImageCommand : ICommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }
}