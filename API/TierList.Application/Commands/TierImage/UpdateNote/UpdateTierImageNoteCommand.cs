using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.TierImage.UpdateNote;

public sealed record UpdateTierImageNoteCommand : ICommand<TierImageDto>
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }

    required public string Note { get; init; }
}