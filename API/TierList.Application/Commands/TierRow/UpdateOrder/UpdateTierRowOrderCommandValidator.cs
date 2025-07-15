using FluentValidation;

namespace TierList.Application.Commands.TierRow.UpdateOrder;

public sealed class UpdateTierRowOrderCommandValidator : AbstractValidator<UpdateTierRowOrderCommand>
{
    public UpdateTierRowOrderCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0).WithMessage("Row ID must be greater than zero.");

        RuleFor(command => command.ListId)
            .GreaterThan(0).WithMessage("List ID must be greater than zero.");

        RuleFor(command => command.Order)
            .GreaterThan(0).WithMessage("Order must be greater than zero.");
    }
}
