namespace TierList.Application.Queries;

public record GetTierImageUploadUrlQuery
{
    required public string FileName { get; init; }

    required public string ContentType { get; init; }
}
