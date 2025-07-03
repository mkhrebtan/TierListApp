using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TierList.Application.Common.DTOs;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Infrastructure.Settings;

namespace TierList.Infrastructure.Services;

public class StorageService : IImageStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3Settings> _s3Settings;

    public StorageService(IAmazonS3 s3Client, IOptions<S3Settings> s3Settings)
    {
        _s3Client = s3Client;
        _s3Settings = s3Settings;
    }

    public async Task<TierImageResult> GetImageUploadUrlAsync(string fileName, string contentType)
    {
        try
        {
            var key = Guid.NewGuid();
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _s3Settings.Value.BucketName,
                Key = $"images/{key}",
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddHours(5),
                ContentType = contentType,
                Metadata =
                {
                    ["file-name"] = fileName,
                },
            };

            string url = await _s3Client.GetPreSignedURLAsync(request);

            return TierImageResult.Success(new TierImageBriefDto
            {
                Url = url,
                StorageKey = key,
            });
        }
        catch (AmazonS3Exception ex)
        {
            return TierImageResult.Failure(ex.Message, ErrorType.UnexpectedError);
        }
    }

    public async Task<TierImageResult> DeleteImageAsync(Guid key)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _s3Settings.Value.BucketName,
                Key = $"images/{key}",
            };

            await _s3Client.DeleteObjectAsync(request);
            return TierImageResult.Success();
        }
        catch (AmazonS3Exception ex)
        {
            return TierImageResult.Failure(ex.Message, ErrorType.UnexpectedError);
        }
    }
}
