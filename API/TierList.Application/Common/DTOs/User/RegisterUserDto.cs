namespace TierList.Application.Common.DTOs.User;

public class RegisterUserDto
{
    public int Id { get; init; }

    required public string UserName { get; init; }
}