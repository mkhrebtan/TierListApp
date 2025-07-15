using FluentValidation;

namespace TierList.Application.Commands.TierImage.Save;

public sealed class SaveTierImageCommandValidator : AbstractValidator<SaveTierImageCommand>
{
    public SaveTierImageCommandValidator()
    {
        RuleFor(command => command.Url)
            .NotEmpty().WithMessage("Image URL cannot be empty.");

        RuleFor(command => command.StorageKey)
            .NotEqual(Guid.Empty).WithMessage("Storage key must be a valid GUID.");

        RuleFor(command => command.Order)
            .GreaterThanOrEqualTo(1).WithMessage("Order must be a positive integer.");

        RuleFor(command => command.ContainerId)
            .GreaterThan(0).WithMessage("Invalid container ID provided.");

        RuleFor(command => command.ListId)
            .GreaterThan(0).WithMessage("Invalid list ID provided.");
    }
}
