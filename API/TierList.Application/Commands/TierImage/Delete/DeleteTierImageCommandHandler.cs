using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Common.Services;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Commands.TierImage.Delete;

internal sealed class DeleteTierImageCommandHandler : ICommandHandler<DeleteTierImageCommand>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageStorageService _imageStorageService;

    public DeleteTierImageCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork,
        IImageStorageService imageStorageService)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result> Handle(DeleteTierImageCommand command)
    {
        if (command.Id <= 0)
        {
            return Result.Failure(
                new Error("Validation", "Invalid image ID provided."));
        }
        else if (command.ListId <= 0)
        {
            return Result.Failure(
                new Error("Validation", "Invalid list ID provided."));
        }
        else if (command.ContainerId <= 0)
        {
            return Result.Failure(
                new Error("Validation", "Invalid container ID provided."));
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(command.ListId);
        if (listEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"List with ID {command.ListId} does not exist."));
        }

        TierImageContainer? containerEntity = await _tierListRepository.GetContainerByIdAsync(command.ContainerId);
        if (containerEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"Container with ID {command.ContainerId} does not exist."));
        }
        else if (containerEntity.TierListId != command.ListId)
        {
            return Result.Failure(
                new Error("Validation", $"Container with ID {command.ContainerId} does not belong to list {command.ListId}."));
        }

        List<TierImageEntity> containerImages = (await _tierListRepository.GetImagesAsync(command.ContainerId)).OrderBy(i => i.Order).ToList();
        TierImageEntity? imageEntity = containerImages.FirstOrDefault(i => i.Id == command.Id);
        if (imageEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"Image with ID {command.Id} does not exist in container {command.ContainerId}."));
        }

        var s3DeleteResult = await _imageStorageService.DeleteImageAsync(imageEntity.StorageKey);
        if (!s3DeleteResult.IsSuccess)
        {
            return s3DeleteResult;
        }

        containerImages.Remove(imageEntity);
        for (int i = 0; i < containerImages.Count; i++)
        {
            containerImages[i].Order = i + 1;
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.DeleteImage(imageEntity);
            foreach (var image in containerImages)
            {
                _tierListRepository.UpdateImage(image);
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return Result.Failure(
                new Error("DatabaseError", $"An error occurred while deleting the image: {ex.Message}"));
        }

        return Result.Success();
    }
}