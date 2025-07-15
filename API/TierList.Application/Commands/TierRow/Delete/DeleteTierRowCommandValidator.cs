using FluentValidation;

namespace TierList.Application.Commands.TierRow.Delete;

public sealed class DeleteTierRowCommandValidator : AbstractValidator<DeleteTierRowCommand>
{
    public DeleteTierRowCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Row ID must be greater than zero.");

        RuleFor(x => x.ListId)
            .GreaterThan(0)
            .WithMessage("List ID must be greater than zero.");
    }
}
