using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.JwtUser.RefreshToken;

public sealed record RefreshTokenCommand : ICommand<LoginUserDto>
{
    required public string RefreshToken { get; init; }
}