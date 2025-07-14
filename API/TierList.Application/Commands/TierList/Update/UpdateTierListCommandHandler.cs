using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierList.Update;

internal sealed class UpdateTierListCommandHandler : ICommandHandler<UpdateTierListCommand, TierListBriefDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTierListCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierListBriefDto>> Handle(UpdateTierListCommand command)
    {
        if (command.Id <= 0)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "Invalid list ID provided."));
        }
        else if (string.IsNullOrEmpty(command.Title))
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "List title cannot be empty."));
        }
        else if (command.UserId <= 0)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "Invalid user ID provided."));
        }
        else if (command.Title.Length > 100)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "List title cannot exceed 100 characters."));
        }

        TierListEntity? existingList = await _tierListRepository.GetByIdAsync(command.Id);
        if (existingList is null)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("NotFound", $"List with ID {command.Id} not found."));
        }
        else if (existingList.UserId != command.UserId)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", $"List with ID {command.Id} does not belong to user with ID {command.UserId}."));
        }

        existingList.Title = command.Title;
        existingList.LastModified = DateTime.UtcNow;

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.Update(existingList);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.CreateTransactionAsync();
            return Result<TierListBriefDto>.Failure(
                new Error("SaveDataError", ex.Message));
        }

        return Result<TierListBriefDto>.Success(
            new TierListBriefDto
            {
                Id = existingList.Id,
                Title = existingList.Title,
                Created = existingList.Created,
                LastModified = existingList.LastModified,
            });
    }
}