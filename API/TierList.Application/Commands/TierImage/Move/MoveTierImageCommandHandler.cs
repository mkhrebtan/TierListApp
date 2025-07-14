using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierImage.Move;

internal sealed class MoveTierImageCommandHandler : ICommandHandler<MoveTierImageCommand, TierImageDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MoveTierImageCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierImageDto>> Handle(MoveTierImageCommand command)
    {
        if (command.Id <= 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Invalid image ID provided."));
        }
        else if (command.ListId <= 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Invalid list ID provided."));
        }
        else if (command.FromContainerId <= 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Invalid source container ID provided."));
        }
        else if (command.ToContainerId <= 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Invalid target container ID provided."));
        }
        else if (command.Order < 0)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", "Order must be a non-negative integer."));
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"List with ID {command.ListId} does not exist."));
        }

        TierImageContainer? sourceContainer = await _tierListRepository.GetContainerByIdAsync(command.FromContainerId);
        if (sourceContainer is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Source container with ID {command.FromContainerId} not found."));
        }
        else if (sourceContainer.TierListId != command.ListId)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Source container with ID {command.FromContainerId} does not belong to list {command.ListId}."));
        }

        TierImageContainer? targetContainer = await _tierListRepository.GetContainerByIdAsync(command.ToContainerId);
        if (targetContainer is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Target container with ID {command.ToContainerId} not found."));
        }
        else if (targetContainer.TierListId != command.ListId)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Target container with ID {command.ToContainerId} does not belong to list {command.ListId}."));
        }

        List<TierImageEntity> targetContainerImages = await _tierListRepository.GetImagesAsync(command.ToContainerId);
        int imagesCount = targetContainerImages.Count;
        int order = command.Order;
        if (order > imagesCount + 1)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Order {order} exceeds the number of images in the target container {command.ToContainerId}."));
        }

        List<TierImageEntity> sourceContainerImages = await _tierListRepository.GetImagesAsync(command.FromContainerId);
        TierImageEntity? imageEntity = sourceContainerImages.FirstOrDefault(i => i.Id == command.Id);
        if (imageEntity is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Image with ID {command.Id} does not exist in source container {command.FromContainerId}."));
        }

        sourceContainerImages = sourceContainerImages.OrderBy(i => i.Order).ToList();
        sourceContainerImages.Remove(imageEntity);
        for (int i = 0; i < sourceContainerImages.Count; i++)
        {
            sourceContainerImages[i].Order = i + 1;
        }

        imageEntity.ContainerId = command.ToContainerId;

        targetContainerImages = targetContainerImages.OrderBy(i => i.Order).ToList();
        targetContainerImages.Insert(order - 1, imageEntity);
        for (int i = 0; i < imagesCount + 1; i++)
        {
            targetContainerImages[i].Order = i + 1;
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            foreach (var image in sourceContainerImages)
            {
                _tierListRepository.UpdateImage(image);
            }

            foreach (var image in targetContainerImages)
            {
                _tierListRepository.UpdateImage(image);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            return Result<TierImageDto>.Failure(
                new Error("DatabaseError", $"An error occurred while moving the image: {ex.Message}"));
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