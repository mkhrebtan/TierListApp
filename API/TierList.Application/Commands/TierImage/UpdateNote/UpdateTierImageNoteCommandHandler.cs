using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierImage.UpdateNote;

internal sealed class UpdateTierImageNoteCommandHandler : ICommandHandler<UpdateTierImageNoteCommand, TierImageDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTierImageNoteCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierImageDto>> Handle(UpdateTierImageNoteCommand command)
    {
        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierImageDto>.Failure(
                 new Error("NotFound", $"List with ID {command.ListId} does not exist."));
        }

        TierImageContainer? containerEntity = await _tierListRepository.GetContainerByIdAsync(command.ContainerId);
        if (containerEntity is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Container with ID {command.ContainerId} does not exist."));
        }
        else if (containerEntity.TierListId != command.ListId)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Container with ID {command.ContainerId} does not belong to list {command.ListId}."));
        }

        TierImageEntity? imageEntity = await _tierListRepository.GetImageByIdAsync(command.Id);
        if (imageEntity is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Image with ID {command.Id} does not exist."));
        }
        else if (imageEntity.ContainerId != command.ContainerId)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Image with ID {command.Id} does not belong to container {command.ContainerId}."));
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            imageEntity.Note = command.Note;
            _tierListRepository.UpdateImage(imageEntity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result<TierImageDto>.Failure(
                new Error("SaveDataError", ex.Message));
        }

        return Result<TierImageDto>.Success(
            new TierImageDto
            {
                Id = imageEntity.Id,
                StorageKey = imageEntity.StorageKey,
                Url = imageEntity.Url,
                Note = imageEntity.Note,
                ContainerId = imageEntity.ContainerId,
                Order = imageEntity.Order,
            });
    }
}