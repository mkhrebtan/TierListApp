using TierList.Application.Common.Models;

namespace TierList.Application.Common.Interfaces;

public interface IImageStorageService
{
    Task<TierImageResult> GetImageUploadUrlAsync(string fileName, string contentType);

    Task<TierImageResult> DeleteImageAsync(Guid key);
}
