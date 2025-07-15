using TierList.Application.Common.DTOs.User;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.JwtUser.Register;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<RegisterUserDto>> Handle(RegisterUserCommand command)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(command.Username);
        if (existingUser != null)
        {
            return Result<RegisterUserDto>.Failure(
                new Error("Validation", "Username already exists."));
        }

        User user = new()
        {
            Username = command.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
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
            return Result<RegisterUserDto>.Failure(
                new Error("SaveDataError", $"An error occurred while registering the user: {ex.Message}"));
        }

        return Result<RegisterUserDto>.Success(new RegisterUserDto
        {
            Id = user.Id,
            UserName = user.Username,
        });
    }
}