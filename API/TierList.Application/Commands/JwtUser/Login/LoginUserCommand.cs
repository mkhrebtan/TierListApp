﻿using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;

namespace TierList.Application.Commands.JwtUser.Login;

public sealed record LoginUserCommand : ICommand<LoginUserDto>
{
    required public string Username { get; init; }

    required public string Password { get; init; }
}