using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierList.Delete;

internal sealed class DeleteTierListCommandHandler : ICommandHandler<DeleteTierListCommand>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTierListCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTierListCommand command)
    {
        if (command.Id <= 0)
        {
            return Result.Failure(
                new Error("Validation", "Invalid list ID provided."));
        }
        else if (command.UserId <= 0)
        {
            return Result.Failure(
                new Error("Validation", "Invalid user ID provided."));
        }

        TierListEntity? listToDelete = await _tierListRepository.GetByIdAsync(command.Id);
        if (listToDelete is null)
        {
            return Result.Failure(
                new Error("NotFound", $"List with ID {command.Id} not found."));
        }
        else if (listToDelete.UserId != command.UserId)
        {
            return Result.Failure(
                new Error("Validation", $"List with ID {command.Id} does not belong to user with ID {command.UserId}."));
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            _tierListRepository.Delete(listToDelete);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.CreateTransactionAsync();
            return Result.Failure(
                new Error("SaveDataError", ex.Message));
        }

        return Result.Success();
    }
}