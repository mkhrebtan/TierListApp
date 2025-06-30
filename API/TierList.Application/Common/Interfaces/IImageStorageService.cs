using TierList.Application.Common.Models;

namespace TierList.Application.Common.Interfaces;

public interface IImageStorageService
{
    Task<TierImageResult> GetImageDownloadUrlAsync(Guid key);

    Task<TierImageResult> GetImageUploadUrlAsync(string fileName);

    Task<TierImageResult> DeleteImageAsync(Guid key);
}
