using FluentValidation;

namespace TierList.Application.Commands.TierRow.UpdateRank;

public sealed class UpdateTierRowRankCommandValidator : AbstractValidator<UpdateTierRowRankCommand>
{
    public UpdateTierRowRankCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Row ID must be greater than zero.");

        RuleFor(x => x.ListId)
            .GreaterThan(0).WithMessage("List ID must be greater than zero.");

        RuleFor(x => x.Rank)
            .NotEmpty().WithMessage("Rank cannot be empty.")
            .MaximumLength(50).WithMessage("Rank cannot exceed 50 characters.");
    }
}