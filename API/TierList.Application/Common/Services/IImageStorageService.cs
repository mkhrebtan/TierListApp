using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Models;

namespace TierList.Application.Common.Services;

public interface IImageStorageService
{
    Task<Result<TierImageBriefDto>> GetImageUploadUrlAsync(string fileName, string contentType);

    Task<Result> DeleteImageAsync(Guid key);
}
