using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

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
        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"List with ID {command.ListId} not found."));
        }

        TierRowEntity? rowEntity = await _tierListRepository.GetRowByIdAsync(command.Id);
        if (rowEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"Row with ID {command.Id} not found."));
        }

        if (rowEntity.TierListId != command.ListId)
        {
            return Result.Failure(
                new Error("Validation", $"Row with ID {command.Id} does not belong to list with ID {command.ListId}."));
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            if (!command.IsDeleteWithImages)
            {
                IEnumerable<TierImageEntity> rowImages = await _tierListRepository.GetImagesAsync(rowEntity.Id);
                if (rowImages.Any())
                {
                    TierBackupRowEntity? backupRowEntity = await _tierListRepository.GetBackupRowAsync(command.ListId);
                    if (backupRowEntity is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Result.Failure(
                            new Error("NotFound", "Backup row not found for the list."));
                    }

                    int backupRowImagesCount = (await _tierListRepository.GetImagesAsync(backupRowEntity.Id)).Count();
                    foreach (var image in rowImages.OrderBy(i => i.Order).ToList())
                    {
                        image.ContainerId = backupRowEntity.Id;
                        image.Order = ++backupRowImagesCount;
                        _tierListRepository.UpdateImage(image);
                    }
                }
            }

            _tierListRepository.DeleteRow(rowEntity);

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