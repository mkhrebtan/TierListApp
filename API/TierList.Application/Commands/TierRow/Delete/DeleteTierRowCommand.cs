using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierRow.Delete;

public sealed record DeleteTierRowCommand : ICommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public bool IsDeleteWithImages { get; init; }
}