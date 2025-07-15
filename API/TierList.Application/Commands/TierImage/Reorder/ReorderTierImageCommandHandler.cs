using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierImage.Reorder;

internal sealed class ReorderTierImageCommandHandler : ICommandHandler<ReorderTierImageCommand, TierImageDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderTierImageCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierImageDto>> Handle(ReorderTierImageCommand command)
    {
        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierImageDto>.Failure(
                 new Error("NotFound", $"List with ID {command.ListId} does not exist."));
        }

        TierImageContainer? targetContainer = await _tierListRepository.GetContainerByIdAsync(command.ContainerId);
        if (targetContainer is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Container with ID {command.ContainerId} does not exist."));
        }
        else if (targetContainer.TierListId != command.ListId)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Container with ID {command.ContainerId} does not belong to list {command.ListId}."));
        }

        List<TierImageEntity> containerImages = await _tierListRepository.GetImagesAsync(command.ContainerId);
        TierImageEntity? imageEntity = containerImages.FirstOrDefault(i => i.Id == command.Id);
        if (imageEntity is null)
        {
            return Result<TierImageDto>.Failure(
                new Error("NotFound", $"Image with ID {command.Id} does not exist in container {command.ContainerId}."));
        }

        int imagesCount = containerImages.Count;
        int order = command.Order;
        if (order > imagesCount + 1)
        {
            return Result<TierImageDto>.Failure(
                new Error("Validation", $"Order {order} exceeds the number of images in the container {command.ContainerId}."));
        }
        else if (order == imageEntity.Order)
        {
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

        containerImages = containerImages.OrderBy(i => i.Order).ToList();
        containerImages.Remove(imageEntity);
        containerImages.Insert(order - 1, imageEntity);
        for (int i = 0; i < imagesCount; i++)
        {
            containerImages[i].Order = i + 1;
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            foreach (var image in containerImages)
            {
                _tierListRepository.UpdateImage(image);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
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