using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;
using TierList.Domain.ValueObjects;

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
        TierListEntity? listEntity = await _tierListRepository.GetTierListWithDataAsync(command.ListId);
        if (listEntity is null)
        {
            return Result<TierImageDto>.Failure(
                 new Error("NotFound", $"List with ID {command.ListId} does not exist."));
        }

        Result<TierImageEntity> imageEntityResult = listEntity.ReorderImage(command.Id, Order.Create(command.Order).Value);
        if (!imageEntityResult.IsSuccess)
        {
            return Result<TierImageDto>.Failure(imageEntityResult.Error);
        }

        TierImageEntity imageEntity = imageEntityResult.Value;

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.Update(listEntity);
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
                Order = imageEntity.Order.Value,
            });
    }
}