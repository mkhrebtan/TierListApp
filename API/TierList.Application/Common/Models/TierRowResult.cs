using TierList.Application.Common.DTOs;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.Models;

public record TierRowResult
{
    public bool IsSuccess { get; init; }

    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the type of error associated with the current operation, if any.
    /// </summary>
    public ErrorType? ErrorType { get; init; }

    public ITierRowDto? TierRowData { get; init; }

    public static TierRowResult Success(ITierRowDto? data = null) => new() { IsSuccess = true, TierRowData = data ?? null };

    public static TierRowResult Failure(string error, ErrorType errorType) => new() { IsSuccess = false, ErrorMessage = error, ErrorType = errorType };
}
