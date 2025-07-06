using TierList.Application.Common.DTOs;
using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Enums;

namespace TierList.Application.Common.Models;

public record RegisterUserResult
{
    public bool IsSuccess { get; init; }

    public string? ErrorMessage { get; init; }

    public ErrorType? ErrorType { get; init; }

    public RegisterUserDto? RegisterUserDto { get; init; }

    public static RegisterUserResult Success(RegisterUserDto? data = null) => new() { IsSuccess = true, RegisterUserDto = data ?? null };

    public static RegisterUserResult Failure(string error, ErrorType errorType) => new() { IsSuccess = false, ErrorMessage = error, ErrorType = errorType };
}