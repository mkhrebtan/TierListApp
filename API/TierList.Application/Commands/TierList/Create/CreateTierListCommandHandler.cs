using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

namespace TierList.Application.Commands.TierList.Create;

internal sealed class CreateTierListCommandHandler : ICommandHandler<CreateTierListCommand, TierListBriefDto>
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTierListCommandHandler(
        ITierListRepository tierListRepository,
        IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TierListBriefDto>> Handle(CreateTierListCommand command)
    {
        Result<TierListEntity> tierListResult = TierListEntity.Create(
            command.Title,
            command.UserId);
        if (!tierListResult.IsSuccess)
        {
            return Result<TierListBriefDto>.Failure(tierListResult.Error);
        }

        TierListEntity tierList = tierListResult.Value;
        try
        {
            await _unitOfWork.CreateTransactionAsync();

            _tierListRepository.Insert(tierList);
            await _unitOfWork.SaveChangesAsync();

            tierList.InitializeDefaultContainers();
            _tierListRepository.Update(tierList);
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
                Id = tierList.Id,
                Title = tierList.Title,
                Created = tierList.Created,
                LastModified = tierList.LastModified,
            });
    }
}