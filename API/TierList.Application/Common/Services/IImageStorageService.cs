using TierList.Application.Common.DTOs.TierImage;
using TierList.Domain.Shared;

namespace TierList.Application.Common.Services;

public interface IImageStorageService
{
    Task<Result<TierImageBriefDto>> GetImageUploadUrlAsync(string fileName, string contentType);

    Task<Result> DeleteImageAsync(Guid key);
}
