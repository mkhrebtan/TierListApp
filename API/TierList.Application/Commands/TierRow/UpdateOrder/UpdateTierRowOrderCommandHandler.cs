using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierRow.UpdateOrder;

internal sealed class UpdateTierRowOrderCommandHandler : ICommandHandler<UpdateTierRowOrderCommand, TierRowBriefDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTierRowOrderCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierRowBriefDto>> Handle(UpdateTierRowOrderCommand command)
    {
        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"List with ID {command.ListId} not found."));
        }

        IEnumerable<TierRowEntity> tierRowEntities = await _tierListRepository.GetRowsAsync(listEntity.Id);
        TierRowEntity? rowEntity = tierRowEntities.FirstOrDefault(r => r.Id == command.Id);
        if (rowEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"Row with ID {command.Id} not found in list {command.ListId}."));
        }
        else if (command.Order > tierRowEntities.Count())
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", $"Order {command.Order} exceeds the number of rows in the list."));
        }
        else if (command.Order == rowEntity.Order)
        {
            return Result<TierRowBriefDto>.Success(
                new TierRowBriefDto
                {
                    Id = rowEntity.Id,
                    Rank = rowEntity.Rank,
                    ColorHex = rowEntity.ColorHex,
                    Order = rowEntity.Order,
                });
        }

        var orderedRows = tierRowEntities.OrderBy(r => r.Order).ToList();
        orderedRows.Remove(rowEntity);
        orderedRows.Insert(command.Order - 1, rowEntity);
        for (int i = 0; i < orderedRows.Count; i++)
        {
            orderedRows[i].Order = i + 1;
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            foreach (var row in orderedRows)
            {
                _tierListRepository.UpdateRow(row);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result<TierRowBriefDto>.Failure(
                new Error("SaveDataError", $"An error occurred while updating the row order: {ex.Message}"));
        }

        return Result<TierRowBriefDto>.Success(
            new TierRowBriefDto
            {
                Id = rowEntity.Id,
                Rank = rowEntity.Rank,
                ColorHex = rowEntity.ColorHex,
                Order = rowEntity.Order,
            });
    }
}