using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

namespace TierList.Application.Commands.TierRow.Delete;

internal sealed class DeleteTierRowCommandHandler : ICommandHandler<DeleteTierRowCommand>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTierRowCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteTierRowCommand command)
    {
        TierListEntity? listEntity = await _tierListRepository.GetTierListWithDataAsync(command.ListId);
        if (listEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"List with ID {command.ListId} not found."));
        }

        listEntity.RemoveRow(command.Id);

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.Update(listEntity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure(
                new Error("SaveDataError", $"An error occurred while deleting the row: {ex.Message}"));
        }

        return Result.Success();
    }
}