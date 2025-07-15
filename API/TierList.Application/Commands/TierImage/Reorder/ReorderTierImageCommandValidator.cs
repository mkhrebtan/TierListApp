using FluentValidation;

namespace TierList.Application.Commands.TierImage.Reorder;

public sealed class ReorderTierImageCommandValidator : AbstractValidator<ReorderTierImageCommand>
{
    public ReorderTierImageCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Image ID must be a positive integer.");

        RuleFor(x => x.ListId)
            .GreaterThan(0)
            .WithMessage("List ID must be a positive integer.");

        RuleFor(x => x.ContainerId)
            .GreaterThan(0)
            .WithMessage("Container ID must be a positive integer.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Order must be a positive integer.");
    }
}