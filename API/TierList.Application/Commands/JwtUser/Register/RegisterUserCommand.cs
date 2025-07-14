using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.JwtUser.Register;

public sealed record RegisterUserCommand : ICommand<RegisterUserDto>
{
    required public string Username { get; init; }

    required public string Password { get; init; }
}