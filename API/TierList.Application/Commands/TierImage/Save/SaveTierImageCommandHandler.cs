using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierImage.Save;

internal sealed class SaveTierImageCommandHandler : ICommandHandler<SaveTierImageCommand, TierImageDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaveTierImageCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierImageDto>> Handle(SaveTierImageCommand command)
    {
        if (string.IsNullOrEmpty(command.Url))
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Image URL cannot be empty."));
        }
        else if (command.StorageKey == Guid.Empty)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Storage key must be a valid GUID."));
        }
        else if (command.Order < 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Order must be a non-negative integer."));
        }
        else if (command.ContainerId <= 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Invalid container ID provided."));
        }
        else if (command.ListId <= 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Invalid list ID provided."));
        }

        TierImageContainer? container = await _tierListRepository.GetBackupRowAsync(command.ListId);
        if (container is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("UnexpectedError", $"Backup row for list with ID {command.ListId} does not exist."));
        }
        else if (container.Id != command.ContainerId)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Provided container with id {command.ContainerId} was not an id of backup row container"));
        }

        List<TierImageEntity> backupImages = await _tierListRepository.GetImagesAsync(container.Id);
        int imagesCount = backupImages.Count;
        if (command.Order > imagesCount + 1)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Order {command.Order} is out of range for the number of images in container {container.Id}."));
        }

        TierImageEntity imageEntity = new()
        {
            StorageKey = command.StorageKey,
            Url = command.Url,
            Note = command.Note,
            ContainerId = command.ContainerId,
            Order = command.Order,
        };

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.AddImage(imageEntity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.CreateTransactionAsync();
            return Result<TierImageDto>.Failure(
                new Error("SaveDataError", $"An error occurred while saving the image: {ex.Message}"));
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
