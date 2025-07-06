namespace TierList.Application.Common.DTOs.User;

public class LoginUserDto
{
    required public string AccessToken { get; init; }

    required public string RefreshToken { get; init; }

    required public DateTime AccessExpiresAt { get; init; }

    required public DateTime RefreshExpiresAt { get; init; }
}