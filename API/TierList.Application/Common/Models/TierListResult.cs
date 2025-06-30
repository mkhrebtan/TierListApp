using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Common.Models;

public record TierListResult
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Gets the error message associated with the current operation, if any.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets the type of error associated with the current operation, if any.
    /// </summary>
    public ErrorType? ErrorType { get; init; }

    /// <summary>
    /// Gets the tier list data associated with the current instance.
    /// </summary>
    public ITierListDto? TierListData { get; init; }

    /// <summary>
    /// Creates a successful result for a tier list operation.
    /// </summary>
    /// <param name="data">The tier list data associated with the successful result. This parameter is optional and can be null.</param>
    /// <returns>A <see cref="TierListResult"/> instance representing a successful operation, with the specified tier list data
    /// if provided.</returns>
    public static TierListResult Success(ITierListDto? data = null) => new() { IsSuccess = true, TierListData = data ?? null };

    /// <summary>
    /// Creates a failed result with the specified error message and error type.
    /// </summary>
    /// <param name="error">The error message describing the failure. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="errorType">The type of error that occurred, indicating the nature of the failure.</param>
    /// <returns>A <see cref="TierListResult"/> instance representing a failed operation.</returns>
    public static TierListResult Failure(string error, ErrorType errorType) => new() { IsSuccess = false, ErrorMessage = error, ErrorType = errorType };
}