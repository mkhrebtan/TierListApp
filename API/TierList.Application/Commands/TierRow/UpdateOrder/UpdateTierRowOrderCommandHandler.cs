using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

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
        TierListEntity? listEntity = await _tierListRepository.GetTierListWithDataAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"List with ID {command.ListId} not found."));
        }

        Result<TierRowEntity> rowEntityResult = listEntity.UpdateRowOrder(command.Id, Order.Create(command.Order).Value);
        if (!rowEntityResult.IsSuccess)
        {
            return Result<TierRowBriefDto>.Failure(rowEntityResult.Error);
        }

        TierRowEntity rowEntity = rowEntityResult.Value;

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
            return Result<TierRowBriefDto>.Failure(
                new Error("SaveDataError", $"An error occurred while updating the row order: {ex.Message}"));
        }

        return Result<TierRowBriefDto>.Success(
            new TierRowBriefDto
            {
                Id = rowEntity.Id,
                Rank = rowEntity.Rank,
                ColorHex = rowEntity.ColorHex,
                Order = rowEntity.Order.Value,
            });
    }
}