using Microsoft.Extensions.Options;
using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierList.Application.Common.Settings;
using TierList.Domain.Abstraction;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.JwtUser.RefreshToken;

internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, LoginUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings;
    }

    public async Task<Result<LoginUserDto>> Handle(RefreshTokenCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Refresh token cannot be empty."));
        }

        var existingToken = await _userRepository.GetRefreshTokenAsync(command.RefreshToken);
        if (existingToken == null)
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Refresh token not found."));
        }
        else if (existingToken.ExpiresAt < DateTime.UtcNow)
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Refresh token has expired."));
        }
        else if (existingToken.IsRevoked)
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Refresh token has already been revoked."));
        }

        existingToken.IsRevoked = true;
        var user = existingToken.User!;

        var accessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user);

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _userRepository.AddRefreshToken(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            return Result<LoginUserDto>.Failure(
                new Error("SaveDataError", $"An error occurred while refreshing token: {ex.Message}"));
        }

        return Result<LoginUserDto>.Success(new LoginUserDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            AccessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.AccessTokenExpirationMinutes),
            RefreshExpiresAt = newRefreshToken.ExpiresAt,
        });
    }
}
