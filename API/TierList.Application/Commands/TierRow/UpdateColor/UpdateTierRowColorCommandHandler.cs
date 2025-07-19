using System.Text.RegularExpressions;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

namespace TierList.Application.Commands.TierRow;

internal sealed class UpdateTierRowColorCommandHandler : ICommandHandler<UpdateTierRowColorCommand, TierRowBriefDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTierRowColorCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierRowBriefDto>> Handle(UpdateTierRowColorCommand command)
    {
        TierListEntity? listEntity = await _tierListRepository.GetTierListWithDataAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"List with ID {command.ListId} not found."));
        }

        Result<TierRowEntity> rowEntityResult = listEntity.UpdateRowColor(command.Id, command.ColorHex);
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
                new Error("SaveDataError", $"An error occurred while updating the row: {ex.Message}"));
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