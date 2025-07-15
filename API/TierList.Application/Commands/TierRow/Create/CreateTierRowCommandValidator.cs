using FluentValidation;

namespace TierList.Application.Commands.TierRow.Create;

public sealed class CreateTierRowCommandValidator : AbstractValidator<CreateTierRowCommand>
{
    public CreateTierRowCommandValidator()
    {
        RuleFor(x => x.ListId)
            .GreaterThan(0)
            .WithMessage("List ID must be a positive integer.");

        RuleFor(x => x.Rank)
            .NotEmpty()
            .WithMessage("Rank cannot be empty.");

        RuleFor(x => x.ColorHex)
            .NotEmpty()
            .Matches(@"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$")
            .WithMessage("ColorHex must be a valid hexadecimal color code.");

        RuleFor(x => x.Order)
            .GreaterThanOrEqualTo(1)
            .When(x => x.Order.HasValue)
            .WithMessage("Order must be a positive integer or null.");
    }
}