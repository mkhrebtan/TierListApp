using Microsoft.Extensions.Options;
using TierList.Application.Commands.User;
using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Models;
using TierList.Application.Common.Settings;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Common.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public UserService(
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

    public async Task<RegisterUserResult> RegisterUserAsync(RegisterUserCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return RegisterUserResult.Failure("Username and password cannot be empty.", ErrorType.ValidationError);
        }
        else if (request.Username.Length < 3 || request.Username.Length > 50)
        {
            return RegisterUserResult.Failure("Username cannot exceed 50 characters.", ErrorType.ValidationError);
        }
        else if (request.Password.Length < 6 || request.Password.Length > 50)
        {
            return RegisterUserResult.Failure("Password must be between 8 and 100 characters.", ErrorType.ValidationError);
        }

        var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
        if (existingUser != null)
        {
            return RegisterUserResult.Failure("Username already exists.", ErrorType.ValidationError);
        }

        User user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _userRepository.Insert(user);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return RegisterUserResult.Failure($"An error occurred while registering the user: {ex.Message}", ErrorType.SaveDataError);
        }

        return RegisterUserResult.Success(new RegisterUserDto
        {
            Id = user.Id,
            UserName = user.Username,
        });
    }

    public async Task<LoginUserResult> LoginUserAsync(LoginUserCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return LoginUserResult.Failure("Username and password cannot be empty.", ErrorType.ValidationError);
        }
        else if (request.Username.Length < 3 || request.Username.Length > 50)
        {
            return LoginUserResult.Failure("Username cannot exceed 50 characters.", ErrorType.ValidationError);
        }
        else if (request.Password.Length < 6 || request.Password.Length > 50)
        {
            return LoginUserResult.Failure("Password must be between 8 and 100 characters.", ErrorType.ValidationError);
        }

        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return LoginUserResult.Failure("Invalid username or password.", ErrorType.ValidationError);
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
            return LoginUserResult.Failure($"An error occurred while logging in: {ex.Message}", ErrorType.SaveDataError);
        }

        var accessTokenExpiryMinutes = _jwtSettings.Value.AccessTokenExpirationMinutes;

        return LoginUserResult.Success(new LoginUserDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes),
            RefreshExpiresAt = refreshToken.ExpiresAt,
        });
    }

    public async Task<LoginUserResult> RefreshTokenAsync(RefreshTokenCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return LoginUserResult.Failure("Refresh token cannot be empty.", ErrorType.ValidationError);
        }

        var existingToken = await _userRepository.GetRefreshTokenAsync(request.RefreshToken);
        if (existingToken == null)
        {
            return LoginUserResult.Failure("Invalid refresh token.", ErrorType.ValidationError);
        }
        else if (existingToken.ExpiresAt < DateTime.UtcNow)
        {
            return LoginUserResult.Failure("Refresh token has expired.", ErrorType.ValidationError);
        }
        else if (existingToken.IsRevoked)
        {
            return LoginUserResult.Failure("Refresh token has been revoked.", ErrorType.ValidationError);
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
            return LoginUserResult.Failure($"An error occurred while refreshing the token: {ex.Message}", ErrorType.SaveDataError);
        }

        return LoginUserResult.Success(new LoginUserDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken.Token,
            AccessExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.Value.AccessTokenExpirationMinutes),
            RefreshExpiresAt = newRefreshToken.ExpiresAt,
        });
    }
}