using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Enums;

namespace TierList.Application.Common.Models;

public record LoginUserResult
{
    public bool IsSuccess { get; init; }

    public string? ErrorMessage { get; init; }

    public ErrorType? ErrorType { get; init; }

    public LoginUserDto? LoginUserDto { get; init; }

    public static LoginUserResult Success(LoginUserDto? data = null) => new() { IsSuccess = true, LoginUserDto = data ?? null };

    public static LoginUserResult Failure(string error, ErrorType errorType) => new() { IsSuccess = false, ErrorMessage = error, ErrorType = errorType };
}
