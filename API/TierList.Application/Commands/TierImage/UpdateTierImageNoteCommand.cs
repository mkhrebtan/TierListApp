using TierList.Application.Common.Interfaces;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierImage;

public record UpdateTierImageNoteCommand : IUpdateImageCommand
{
    required public int Id { get; init; }

    required public int ListId { get; init; }

    required public int ContainerId { get; init; }

    required public string Note { get; init; }

    public void Update(TierImageEntity imageEntity)
    {
        imageEntity.Note = Note;
    }
}
