using TierList.Application.Commands.TierImage.UpdateNote;
using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierImage.UpdateUrl;

public sealed record UpdateTierImageUrlCommand : ICommand<TierImageDto>
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }

    required public string Url { get; init; }
}