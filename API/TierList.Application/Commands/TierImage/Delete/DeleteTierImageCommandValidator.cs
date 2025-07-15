using FluentValidation;

namespace TierList.Application.Commands.TierImage.Delete;

public sealed class DeleteTierImageCommandValidator : AbstractValidator<DeleteTierImageCommand>
{
    public DeleteTierImageCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Image ID must be greater than 0.");

        RuleFor(x => x.ListId)
            .GreaterThan(0)
            .WithMessage("List ID must be greater than 0.");

        RuleFor(x => x.ContainerId)
            .GreaterThan(0)
            .WithMessage("Container ID must be greater than 0.");
    }
}