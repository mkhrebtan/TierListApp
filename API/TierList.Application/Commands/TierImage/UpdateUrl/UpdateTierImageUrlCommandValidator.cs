using FluentValidation;

namespace TierList.Application.Commands.TierImage.UpdateUrl;

public sealed class UpdateTierImageUrlCommandValidator : AbstractValidator<UpdateTierImageUrlCommand>
{
    public UpdateTierImageUrlCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Image ID must be greater than zero.");

        RuleFor(x => x.ListId)
            .GreaterThan(0).WithMessage("List ID must be greater than zero.");

        RuleFor(x => x.ContainerId)
            .GreaterThan(0).WithMessage("Container ID must be greater than zero.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Image URL cannot be empty.");
    }
}