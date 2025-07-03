using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TierList.Application.Common.Interfaces;
using TierList.Infrastructure.Services;
using TierList.Infrastructure.Settings;

namespace TierList.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<S3Settings>(options =>
        {
            options.BucketName = config.GetSection(S3Settings.SectionName)[nameof(S3Settings.BucketName)];
            options.Region = config.GetSection(S3Settings.SectionName)[nameof(S3Settings.Region)];
        });
        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAWSService<IAmazonS3>();
        services.AddScoped<IImageStorageService, StorageService>();
    }
}
