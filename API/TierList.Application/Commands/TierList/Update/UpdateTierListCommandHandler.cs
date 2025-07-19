using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using TierList.Domain.Shared;

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
        TierListEntity? existingList = await _tierListRepository.GetByIdAsync(command.Id);
        if (existingList is null)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("NotFound", $"List with ID {command.Id} not found."));
        }

        existingList.UpdateTitle(command.Title, command.UserId);

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