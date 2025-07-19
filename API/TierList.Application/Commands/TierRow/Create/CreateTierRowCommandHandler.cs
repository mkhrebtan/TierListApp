using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

namespace TierList.Application.Commands.TierRow.Create;

internal sealed class CreateTierRowCommandHandler : ICommandHandler<CreateTierRowCommand, TierRowBriefDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTierRowCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierRowBriefDto>> Handle(CreateTierRowCommand command)
    {
        TierListEntity? listEntity = await _tierListRepository.GetTierListWithDataAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"Tier list with ID {command.ListId} does not exist."));
        }

        Result<TierRowEntity> newRowResult = listEntity.AddRow(
            command.Rank,
            command.ColorHex,
            Order.Create(command.Order).Value);

        if (!newRowResult.IsSuccess)
        {
            return Result<TierRowBriefDto>.Failure(newRowResult.Error);
        }

        TierRowEntity newRow = newRowResult.Value;

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
                new Error("SaveDataError", $"An error occurred while creating the tier row: {ex.Message}"));
        }

        return Result<TierRowBriefDto>.Success(
            new TierRowBriefDto
            {
                Id = newRow.Id,
                Rank = newRow.Rank,
                ColorHex = newRow.ColorHex,
                Order = newRow.Order.Value,
            });
    }
}