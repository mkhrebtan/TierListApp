using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;

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
        if (string.IsNullOrEmpty(query.FileName) || string.IsNullOrEmpty(query.ContentType))
        {
            return Result<TierImageBriefDto>.Failure(
                new Error("Validation", "File name and content type cannot be empty."));
        }

        return await _imageStorageService.GetImageUploadUrlAsync(query.FileName, query.ContentType);
    }
}
