using TierList.Application.Commands.User;
using TierList.Application.Common.Models;

namespace TierList.Application.Common.Services;

public interface IUserService
{
    Task<RegisterUserResult> RegisterUserAsync(RegisterUserCommand request);

    Task<LoginUserResult> LoginUserAsync(LoginUserCommand request);

    Task<LoginUserResult> RefreshTokenAsync(RefreshTokenCommand request);
}