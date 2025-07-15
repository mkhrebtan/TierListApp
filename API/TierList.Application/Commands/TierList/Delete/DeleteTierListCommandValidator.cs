using FluentValidation;

namespace TierList.Application.Commands.TierList.Delete;

public sealed class DeleteTierListCommandValidator : AbstractValidator<DeleteTierListCommand>
{
    public DeleteTierListCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("List ID must be greater than zero.");
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than zero.");
    }
}
