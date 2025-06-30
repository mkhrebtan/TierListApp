using Amazon.Extensions.NETCore.Setup;

namespace TierList.Infrastructure.Settings;

public sealed class S3Settings
{
    public const string SectionName = "S3Settings";

    public string BucketName { get; set; }

    public S3Settings()
        : this(string.Empty)
    {
    }

    public S3Settings(string bucketName)
    {
        BucketName = bucketName;
    }
}
