using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TierList.Application.Commands;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Queries;
using TierList.Application.Queries.DTOs;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Application.Common.Services;

public class TierListService : ITierListService
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TierListService(ITierListRepository tierListRepository, IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    public TierListResult CreateTierList(CreateTierListCommand request)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            return TierListResult.Failure(error: "List title cannot be empty.", errorType: ErrorType.ValidationError);
        }

        TierListEntity tierList = new ()
        {
            Title = request.Title,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
        };

        List<TierImageContainer> defaultContainers = new ()
        {
            new TierRowEntity { TierListId = tierList.Id, Rank = "A", ColorHex = "#FFBF7F", Order = 1 },
            new TierRowEntity { TierListId = tierList.Id, Rank = "B", ColorHex = "#FFDF7F", Order = 2 },
            new TierRowEntity { TierListId = tierList.Id, Rank = "C", ColorHex = "#FFFF7F", Order = 3 },
            new TierBackupRowEntity { TierListId = tierList.Id },
        };

        tierList.Containers.AddRange(defaultContainers);

        try
        {
            _unitOfWork.CreateTransaction();

            _tierListRepository.Insert(tierList);

            _unitOfWork.SaveChanges();
            _unitOfWork.CommitTransaction();
        }
        catch (InvalidOperationException ex)
        {
            _unitOfWork.RollbackTransaction();
            return TierListResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }
        catch (Exception)
        {
            _unitOfWork.RollbackTransaction();
            return TierListResult.Failure(error: "An unexpected error occurred while creating the tier list.", ErrorType.UnexpectedError);
        }

        return TierListResult.Success(
            new TierListBriefDTO
            {
                Id = tierList.Id,
                Title = tierList.Title,
                Created = tierList.Created,
                LastModified = tierList.LastModified,
            });
    }

    public TierListResult DeleteTierList(DeleteTierListCommand request)
    {
        if (request.Id <= 0)
        {
            return TierListResult.Failure(error: "Invalid list ID provided.", errorType: ErrorType.ValidationError);
        }

        TierListEntity? listToDelete = _tierListRepository.GetById(request.Id);
        if (listToDelete is null)
        {
           return TierListResult.Failure(error: $"List with ID {request.Id} not found.", errorType: ErrorType.NotFound);
        }

        try
        {
            _unitOfWork.CreateTransaction();

            _tierListRepository.Delete(listToDelete);

            _unitOfWork.SaveChanges();
            _unitOfWork.CommitTransaction();
        }
        catch (InvalidOperationException ex)
        {
            _unitOfWork.RollbackTransaction();
            return TierListResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }
        catch (Exception)
        {
            _unitOfWork.RollbackTransaction();
            return TierListResult.Failure(error: "An unexpected error occurred while deleting the tier list.", ErrorType.UnexpectedError);
        }

        return TierListResult.Success();
    }

    public IReadOnlyCollection<TierListBriefDTO> GetTierLists(GetTierListsQuery request)
    {
        return _tierListRepository.GetAll()
            .Select(tierList => new TierListBriefDTO
            {
                Id = tierList.Id,
                Title = tierList.Title,
                Created = tierList.Created,
                LastModified = tierList.LastModified,
            })
            .ToList()
            .AsReadOnly();
    }

    public TierListResult UpdateTierList(UpdateTierListCommand request)
    {
        if (request.Id <= 0)
        {
            return TierListResult.Failure(error: "Invalid list ID provided.", errorType: ErrorType.ValidationError);
        }
        else if (string.IsNullOrEmpty(request.Title))
        {
            return TierListResult.Failure(error: "List title cannot be empty.", errorType: ErrorType.ValidationError);
        }

        TierListEntity? existingList = _tierListRepository.GetById(request.Id);
        if (existingList is null)
        {
            return TierListResult.Failure(error: $"List with ID {request.Id} not found.", errorType: ErrorType.NotFound);
        }

        existingList.Title = request.Title;
        existingList.LastModified = DateTime.UtcNow;

        try
        {
            _unitOfWork.CreateTransaction();
            _tierListRepository.Update(existingList);
            _unitOfWork.SaveChanges();
            _unitOfWork.CommitTransaction();
        }
        catch (InvalidOperationException ex)
        {
            _unitOfWork.RollbackTransaction();
            return TierListResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }
        catch (Exception)
        {
            _unitOfWork.RollbackTransaction();
            return TierListResult.Failure(error: "An unexpected error occurred while updating the tier list.", ErrorType.UnexpectedError);
        }

        return TierListResult.Success(
            new TierListBriefDTO
            {
                Id = existingList.Id,
                Title = existingList.Title,
                Created = existingList.Created,
                LastModified = existingList.LastModified,
            });
    }

    public TierListResult GetTierListData(GetTierListDataQuery request)
    {
        if (request.Id <= 0)
        {
            return TierListResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? tierList = _tierListRepository.GetById(request.Id);
        if (tierList is null)
        {
            return TierListResult.Failure(
                error: $"List with ID {request.Id} not found.",
                errorType: ErrorType.NotFound);
        }

        IReadOnlyCollection<TierRowDTO> listRows = _tierListRepository.GetRows(tierList.Id)
            .Select(r => new TierRowDTO
            {
                Id = r.Id,
                Rank = r.Rank,
                ColorHex = r.ColorHex,
                Order = r.Order,
                Images = r.Images.Select(i => new TierImageDTO
                {
                    Id = i.Id,
                    Url = i.Url,
                    Note = i.Note,
                    ContainerId = i.ContainerId,
                    Order = i.Order,
                }).OrderBy(i => i.Order).ToList().AsReadOnly(),
            }).OrderBy(r => r.Order).ToList().AsReadOnly();

        TierBackupRowEntity? listBackupRowEntity = _tierListRepository.GetBackupRow(tierList.Id);
        if (listBackupRowEntity is null)
        {
            return TierListResult.Failure(
                error: $"Backup row for list with ID {request.Id} not found.",
                errorType: ErrorType.UnexpectedError);
        }

        TierListDataDTO tierListData = new ()
        {
            Id = tierList.Id,
            Title = tierList.Title,
            Rows = listRows,
            BackupRow = new TierBackupRowDTO
            {
                Id = listBackupRowEntity.Id,
                Images = listBackupRowEntity.Images
                    .Select(i => new TierImageDTO
                    {
                        Id = i.Id,
                        Url = i.Url,
                        Note = i.Note,
                        ContainerId = i.ContainerId,
                        Order = i.Order,
                    }).OrderBy(i => i.Order).ToList().AsReadOnly(),
            },
        };

        return TierListResult.Success(tierListData);
    }
}
