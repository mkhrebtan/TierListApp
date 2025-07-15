using FluentValidation;

namespace TierList.Application.Queries.GetUploadUrl;

public sealed class GetTierImageUploadUrlQueryValidator : AbstractValidator<GetTierImageUploadUrlQuery>
{
    public GetTierImageUploadUrlQueryValidator()
    {
        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name cannot be empty.")
            .MaximumLength(255).WithMessage("File name cannot exceed 255 characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type cannot be empty.")
            .Matches(@"^image\/[a-zA-Z]+$").WithMessage("Content type must be a valid image MIME type.");
    }
}
