using FluentValidation;

namespace TierList.Application.Commands.TierRow;

public sealed class UpdateTierRowColorCommandValidator : AbstractValidator<UpdateTierRowColorCommand>
{
    public UpdateTierRowColorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Row ID must be greater than zero.");

        RuleFor(x => x.ListId)
            .GreaterThan(0)
            .WithMessage("List ID must be greater than zero.");

        RuleFor(x => x.ColorHex)
            .NotEmpty()
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color hex must be in the format #RRGGBB or #RGB.");
    }
}