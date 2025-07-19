using FluentValidation;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierImage.UpdateNote;

public sealed class UpdateTierImageNoteCommandValidator : AbstractValidator<UpdateTierImageNoteCommand>
{
    public UpdateTierImageNoteCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Image ID must be greater than zero.");

        RuleFor(x => x.ListId)
            .GreaterThan(0).WithMessage("List ID must be greater than zero.");

        RuleFor(x => x.ContainerId)
            .GreaterThan(0).WithMessage("Container ID must be greater than zero.");

        int maxNoteLength = TierImageEntity.MaxNoteLength;
        RuleFor(x => x.Note)
            .MaximumLength(maxNoteLength)
            .WithMessage($"Note must not exceed {maxNoteLength} characters.");
    }
}