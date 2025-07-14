using Microsoft.Extensions.Options;
using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierList.Application.Common.Settings;
using TierList.Domain.Abstraction;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.JwtUser.Login;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public LoginUserCommandHandler(
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

    public async Task<Result<LoginUserDto>> Handle(LoginUserCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Username) || string.IsNullOrWhiteSpace(command.Password))
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Username and password cannot be empty."));
        }
        else if (command.Username.Length < 3 || command.Username.Length > 50)
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Username must be between 3 and 50 characters."));
        }
        else if (command.Password.Length < 6 || command.Password.Length > 50)
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Password must be between 8 and 100 characters."));
        }

        var user = await _userRepository.GetByUsernameAsync(command.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
        {
            return Result<LoginUserDto>.Failure(
                new Error("Validation", "Invalid username or password."));
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _userRepository.AddRefreshToken(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            return Result<LoginUserDto>.Failure(
                new Error("SaveDataError", $"An error occurred while logging in: {ex.Message}"));
        }

        var accessTokenExpiryMinutes = _jwtSettings.Value.AccessTokenExpirationMinutes;

        return Result<LoginUserDto>.Success(new LoginUserDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes),
            RefreshExpiresAt = refreshToken.ExpiresAt,
        });
    }
}
