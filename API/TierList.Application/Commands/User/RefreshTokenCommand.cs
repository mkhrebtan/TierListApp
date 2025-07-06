namespace TierList.Application.Commands.User;

public record RefreshTokenCommand
{
    required public string RefreshToken { get; init; }
}
