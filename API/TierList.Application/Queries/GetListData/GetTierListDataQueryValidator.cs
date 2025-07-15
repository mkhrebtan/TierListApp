using FluentValidation;

namespace TierList.Application.Queries.GetListData;

public sealed class GetTierListDataQueryValidator : AbstractValidator<GetTierListDataQuery>
{
    public GetTierListDataQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("List ID must be greater than 0.");

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID must be greater than 0.");
    }
}