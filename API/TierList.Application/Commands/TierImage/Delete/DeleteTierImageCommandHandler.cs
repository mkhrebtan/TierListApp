using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Services;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

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
        TierListEntity? listEntity = await _tierListRepository.GetTierListWithDataAsync(command.ListId);
        if (listEntity is null)
        {
            return Result.Failure(
                new Error("NotFound", $"List with ID {command.ListId} does not exist."));
        }

        Result<TierImageEntity> imageEntityResult = listEntity.RemoveImage(command.Id);
        if (!imageEntityResult.IsSuccess)
        {
            return Result.Failure(imageEntityResult.Error);
        }

        var s3DeleteResult = await _imageStorageService.DeleteImageAsync(imageEntityResult.Value.StorageKey);
        if (!s3DeleteResult.IsSuccess)
        {
            return s3DeleteResult;
        }

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
            return Result.Failure(
                new Error("DatabaseError", $"An error occurred while deleting the image: {ex.Message}"));
        }

        return Result.Success();
    }
}