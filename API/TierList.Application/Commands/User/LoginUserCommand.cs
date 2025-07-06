namespace TierList.Application.Commands.User;

public record LoginUserCommand
{
    required public string Username { get; init; }

    required public string Password { get; init; }
}
