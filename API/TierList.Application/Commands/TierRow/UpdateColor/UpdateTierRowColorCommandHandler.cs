using System.Text.RegularExpressions;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

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
        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"List with ID {command.ListId} not found."));
        }

        TierRowEntity? rowEntity = await _tierListRepository.GetRowByIdAsync(command.Id);
        if (rowEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"Row with ID {command.Id} not found."));
        }

        if (command.ListId != rowEntity.TierListId)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "Row does not belong to the specified list."));
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            rowEntity.ColorHex = command.ColorHex;
            _tierListRepository.UpdateRow(rowEntity);
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
            Order = rowEntity.Order,
        });
    }
}