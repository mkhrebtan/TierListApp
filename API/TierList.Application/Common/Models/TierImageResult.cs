using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.Models;

public record TierImageResult
{
    public bool IsSuccess { get; init; }

    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the type of error associated with the current operation, if any.
    /// </summary>
    public ErrorType? ErrorType { get; init; }

    public ITierImageDto? TierImageData { get; init; }

    public static TierImageResult Success(ITierImageDto? data = null) => new() { IsSuccess = true, TierImageData = data ?? null };

    public static TierImageResult Failure(string error, ErrorType errorType) => new() { IsSuccess = false, ErrorMessage = error, ErrorType = errorType };
}