using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Services;
using TierList.Domain.Shared;

namespace TierList.Application.Queries.GetUploadUrl;

internal sealed class GetTierImageUploadUrlQueryHandler : IQueryHandler<GetTierImageUploadUrlQuery, TierImageBriefDto>
{
    private readonly IImageStorageService _imageStorageService;

    public GetTierImageUploadUrlQueryHandler(IImageStorageService imageStorageService)
    {
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<TierImageBriefDto>> Handle(GetTierImageUploadUrlQuery query)
    {
        return await _imageStorageService.GetImageUploadUrlAsync(query.FileName, query.ContentType);
    }
}