using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Repos;

namespace TierList.Application.Queries.GetUploadUrl;

public sealed record GetTierImageUploadUrlQuery : IQuery<TierImageBriefDto>
{
    required public string FileName { get; init; }

    required public string ContentType { get; init; }
}