using TierList.Application.Common.Models;

namespace TierList.Application.Common.Services;

public interface IImageStorageService
{
    Task<TierImageResult> GetImageUploadUrlAsync(string fileName, string contentType);

    Task<TierImageResult> DeleteImageAsync(Guid key);
}
