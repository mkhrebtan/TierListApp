using FluentValidation;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierList.Update;

public sealed class UpdateTierListCommandValidator : AbstractValidator<UpdateTierListCommand>
{
    public UpdateTierListCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Invalid list ID provided.");

        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage("Invalid user ID provided.");

        RuleFor(command => command.Title)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("List title cannot be empty.")
            .MaximumLength(TierListEntity.MaxTitleLength)
            .WithMessage("List title cannot exceed 100 characters.");
    }
}