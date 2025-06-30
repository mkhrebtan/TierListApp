namespace TierList.Application.Queries;

public record GetTierImageUploadUrlQuery
{
    required public string FileName { get; init; }
}
