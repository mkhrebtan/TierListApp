using FluentValidation;
using TierList.Domain.Entities;

namespace TierList.Application.Commands.TierList.Create;

public sealed class CreateTierListCommandValidator : AbstractValidator<CreateTierListCommand>
{
    public CreateTierListCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty().WithMessage("List title cannot be empty.")
            .MaximumLength(TierListEntity.MaxTitleLength).WithMessage("List title cannot exceed 100 characters.");

        RuleFor(command => command.UserId)
            .GreaterThan(0).WithMessage("Invalid user ID provided.");
    }
}