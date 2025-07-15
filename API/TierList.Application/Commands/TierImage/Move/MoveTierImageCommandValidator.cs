using FluentValidation;

namespace TierList.Application.Commands.TierImage.Move;

public sealed class MoveTierImageCommandValidator : AbstractValidator<MoveTierImageCommand>
{
    public MoveTierImageCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("Image ID must be greater than 0.");

        RuleFor(command => command.ListId)
            .GreaterThan(0).WithMessage("List ID must be greater than 0.");

        RuleFor(command => command.FromContainerId)
            .GreaterThan(0).WithMessage("Source container ID must be greater than 0.");

        RuleFor(command => command.ToContainerId)
            .GreaterThan(0).WithMessage("Target container ID must be greater than 0.");

        RuleFor(command => command.Order)
            .GreaterThanOrEqualTo(1).WithMessage("Order must be a positive integer.");
    }
}