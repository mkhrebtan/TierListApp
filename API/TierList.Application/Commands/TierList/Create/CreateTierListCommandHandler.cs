using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

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
        if (string.IsNullOrEmpty(command.Title))
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "List title cannot be empty."));
        }
        else if (command.Title.Length > 100)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "List title cannot exceed 100 characters."));
        }
        else if (command.UserId <= 0)
        {
            return Result<TierListBriefDto>.Failure(
                new Error("Validation", "Invalid user ID provided."));
        }

        TierListEntity tierList = new()
        {
            Title = command.Title,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            UserId = command.UserId,
        };

        List<TierImageContainer> defaultContainers = new()
        {
            new TierRowEntity { TierListId = tierList.Id, Rank = "A", ColorHex = "#FFBF7F", Order = 1 },
            new TierRowEntity { TierListId = tierList.Id, Rank = "B", ColorHex = "#FFDF7F", Order = 2 },
            new TierRowEntity { TierListId = tierList.Id, Rank = "C", ColorHex = "#FFFF7F", Order = 3 },
            new TierBackupRowEntity { TierListId = tierList.Id },
        };

        tierList.Containers.AddRange(defaultContainers);

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            _tierListRepository.Insert(tierList);

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