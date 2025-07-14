using System.Text.RegularExpressions;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

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
        if (command.ListId <= 0)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "Invalid list ID provided."));
        }
        else if (string.IsNullOrEmpty(command.Rank))
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "Rank cannot be empty."));
        }
        else if (string.IsNullOrEmpty(command.ColorHex))
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "ColorHex cannot be empty."));
        }
        else if (command.Order is not null && command.Order <= 0)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "Order must be a positive integer or null."));
        }

        Regex colorHexRegex = new(@"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$");
        if (!colorHexRegex.IsMatch(command.ColorHex))
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "ColorHex must be a valid hexadecimal color code."));
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("NotFound", $"Tier list with ID {command.ListId} does not exist."));
        }

        int rowsCount = (await _tierListRepository.GetRowsAsync(command.ListId)).Count();
        if (command.Order is not null && command.Order > rowsCount + 1)
        {
            return Result<TierRowBriefDto>.Failure(
                new Error("Validation", "Order cannot be greater than the number of existing rows plus one."));
        }

        TierRowEntity newRow = new()
        {
            TierListId = command.ListId,
            Rank = command.Rank,
            ColorHex = command.ColorHex,
            Order = command.Order ?? rowsCount + 1,
        };

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.AddRow(newRow);
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
                Order = newRow.Order,
            });
    }
}
