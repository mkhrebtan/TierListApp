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

/// <summary>
/// Provides functionality for managing tier lists, including creating, updating, deleting,  and retrieving tier list
/// data. This service acts as the primary interface for interacting  with tier list entities and their associated data.
/// </summary>
/// <remarks>The <see cref="TierListService"/> class implements the <see cref="ITierListService"/> interface  and
/// provides methods for performing CRUD operations on tier lists. It ensures transactional  consistency and handles
/// validation and error scenarios, such as invalid input or missing data.  This service relies on an <see
/// cref="ITierListRepository"/> for data access and an  <see cref="IUnitOfWork"/> for managing transactions. It is
/// designed to encapsulate business  logic and enforce domain rules related to tier lists.</remarks>
public class TierListService : ITierListService
{
    private readonly ITierListRepository _tierListRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="TierListService"/> class.
    /// </summary>
    /// <param name="tierListRepository">The repository used to manage tier list data. This parameter cannot be null.</param>
    /// <param name="unitOfWork">The unit of work instance used to manage transactional operations. This parameter cannot be null.</param>
    public TierListService(ITierListRepository tierListRepository, IUnitOfWork unitOfWork)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new tier list with default tier rows and saves it to the repository.
    /// </summary>
    /// <remarks>The method validates the input to ensure the title is not null or empty.  If the validation
    /// fails, the operation returns a failure result with a validation error. Default tier rows are added to the tier
    /// list before saving.</remarks>
    /// <param name="request">The command containing the details for the tier list to be created, including its title.</param>
    /// <returns>A <see cref="TierListResult"/> indicating the success or failure of the operation.  On success, the result
    /// contains a brief DTO of the created tier list.  On failure, the result includes an error message and error type.</returns>
    public async Task<TierListResult> CreateTierListAsync(CreateTierListCommand request)
    {
        if (string.IsNullOrEmpty(request.Title))
        {
            return TierListResult.Failure(error: "List title cannot be empty.", errorType: ErrorType.ValidationError);
        }

        TierListEntity tierList = new()
        {
            Title = request.Title,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
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
            return TierListResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierListResult.Success(
            new TierListBriefDto
            {
                Id = tierList.Id,
                Title = tierList.Title,
                Created = tierList.Created,
                LastModified = tierList.LastModified,
            });
    }

    /// <summary>
    /// Deletes a tier list based on the specified request.
    /// </summary>
    /// <remarks>This method performs validation on the provided ID and ensures the tier list exists before
    /// attempting deletion.  If an error occurs during the transaction, the operation is rolled back, and an
    /// appropriate error result is returned.</remarks>
    /// <param name="request">The command containing the ID of the tier list to delete. The ID must be greater than zero.</param>
    /// <returns>A <see cref="TierListResult"/> indicating the outcome of the operation.  Returns a success result if the tier
    /// list is deleted successfully.  Returns a failure result if the ID is invalid, the tier list is not found, or an
    /// error occurs during the deletion process.</returns>
    public async Task<TierListResult> DeleteTierListAsync(DeleteTierListCommand request)
    {
        if (request.Id <= 0)
        {
            return TierListResult.Failure(error: "Invalid list ID provided.", errorType: ErrorType.ValidationError);
        }

        TierListEntity? listToDelete = await _tierListRepository.GetByIdAsync(request.Id);
        if (listToDelete is null)
        {
            return TierListResult.Failure(error: $"List with ID {request.Id} not found.", errorType: ErrorType.NotFound);
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            _tierListRepository.Delete(listToDelete);

            await _unitOfWork.CreateTransactionAsync();
            await _unitOfWork.CreateTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.CreateTransactionAsync();
            return TierListResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierListResult.Success();
    }

    /// <summary>
    /// Retrieves a collection of tier lists based on the specified query.
    /// </summary>
    /// <remarks>The returned collection includes basic details about each tier list, such as its ID, title,
    /// creation date,  and last modification date. The query parameters in <paramref name="request"/> may influence the
    /// retrieval  process, depending on the implementation of the repository.</remarks>
    /// <param name="request">The query parameters used to filter or modify the retrieval of tier lists.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only collection  of <see
    /// cref="TierListBriefDto"/> objects, each representing a brief summary of a tier list.</returns>
    public async Task<IReadOnlyCollection<TierListBriefDto>> GetTierListsAsync(GetTierListsQuery request)
    {
        var tierListsEntities = await _tierListRepository.GetAllAsync();
        IEnumerable<Task<TierListBriefDto>> mapListToDtoTasks = tierListsEntities
            .Select(t => Task.Run(() => new TierListBriefDto
            {
                Id = t.Id,
                Title = t.Title,
                Created = t.Created,
                LastModified = t.LastModified,
            }));
        return (await Task.WhenAll(mapListToDtoTasks)).AsReadOnly();
    }

    /// <summary>
    /// Updates an existing tier list with the specified details.
    /// </summary>
    /// <remarks>The method validates the input request to ensure the list ID is greater than zero and the
    /// title is not null or empty.  If the specified tier list does not exist, a "Not Found" error is returned.  The
    /// method performs the update within a transaction to ensure data consistency.</remarks>
    /// <param name="request">The command containing the details for updating the tier list, including the list ID and the new title.</param>
    /// <returns>A <see cref="TierListResult"/> indicating the outcome of the operation.  If successful, the result contains a
    /// brief DTO of the updated tier list.  If the operation fails, the result contains an error message and error
    /// type.</returns>
    public async Task<TierListResult> UpdateTierListAsync(UpdateTierListCommand request)
    {
        if (request.Id <= 0)
        {
            return TierListResult.Failure(error: "Invalid list ID provided.", errorType: ErrorType.ValidationError);
        }
        else if (string.IsNullOrEmpty(request.Title))
        {
            return TierListResult.Failure(error: "List title cannot be empty.", errorType: ErrorType.ValidationError);
        }

        TierListEntity? existingList = await _tierListRepository.GetByIdAsync(request.Id);
        if (existingList is null)
        {
            return TierListResult.Failure(error: $"List with ID {request.Id} not found.", errorType: ErrorType.NotFound);
        }

        existingList.Title = request.Title;
        existingList.LastModified = DateTime.UtcNow;

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.Update(existingList);
            await _unitOfWork.CreateTransactionAsync();
            await _unitOfWork.CreateTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.CreateTransactionAsync();
            return TierListResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierListResult.Success(
            new TierListBriefDto
            {
                Id = existingList.Id,
                Title = existingList.Title,
                Created = existingList.Created,
                LastModified = existingList.LastModified,
            });
    }

    /// <summary>
    /// Retrieves tier list data, including rows and backup row information, for the specified tier list ID.
    /// </summary>
    /// <remarks>This method validates the provided tier list ID and retrieves the associated tier list data,
    /// including its rows and backup row. If the tier list or backup row cannot be found, an appropriate error result
    /// is returned.</remarks>
    /// <param name="request">The query containing the ID of the tier list to retrieve. The ID must be greater than 0.</param>
    /// <returns>A <see cref="TierListResult"/> containing the tier list data if the operation is successful, or an error result
    /// if the tier list is not found, the backup row is missing, or the ID is invalid.</returns>
    public async Task<TierListResult> GetTierListDataAsync(GetTierListDataQuery request)
    {
        if (request.Id <= 0)
        {
            return TierListResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? tierList = await _tierListRepository.GetByIdAsync(request.Id);
        if (tierList is null)
        {
            return TierListResult.Failure(
                error: $"List with ID {request.Id} not found.",
                errorType: ErrorType.NotFound);
        }

        TierBackupRowEntity? listBackupRowEntity = await _tierListRepository.GetBackupRowAsync(tierList.Id);
        if (listBackupRowEntity is null)
        {
            return TierListResult.Failure(
                error: $"Backup row for list with ID {request.Id} not found.",
                errorType: ErrorType.UnexpectedError);
        }

        IReadOnlyCollection<TierRowDto> listRows = await GetTierRowDtosAsync(tierList);
        TierBackupRowDto listBackupRowDto = GetTierBackupRowDto(listBackupRowEntity);

        TierListDataDto tierListData = new()
        {
            Id = tierList.Id,
            Title = tierList.Title,
            Rows = listRows,
            BackupRow = listBackupRowDto,
        };

        return TierListResult.Success(tierListData);
    }

    private async Task<IReadOnlyCollection<TierRowDto>> GetTierRowDtosAsync(TierListEntity listEntity)
    {
        IEnumerable<TierRowEntity> rowEntities = await _tierListRepository.GetRowsAsync(listEntity.Id);
        List<TierRowDto> tierRowDtos = new();

        foreach (var rowEntity in rowEntities)
        {
            List<TierImageDto> rowImages = new();
            if (rowEntity.Images.Any())
            {
                foreach (var image in rowEntity.Images)
                {
                    rowImages.Add(new TierImageDto
                    {
                        Id = image.Id,
                        Url = image.Url,
                        Note = image.Note,
                        ContainerId = image.ContainerId,
                        Order = image.Order,
                    });
                }

                rowImages = rowImages.OrderBy(i => i.Order).ToList();
            }

            tierRowDtos.Add(new TierRowDto
            {
                Id = rowEntity.Id,
                Rank = rowEntity.Rank,
                ColorHex = rowEntity.ColorHex,
                Order = rowEntity.Order,
                Images = rowImages.AsReadOnly(),
            });
        }

        return tierRowDtos.OrderBy(r => r.Order).ToList().AsReadOnly();
    }

    private TierBackupRowDto GetTierBackupRowDto(TierBackupRowEntity backupRowEntity)
    {
        List<TierImageDto> backupImages = new();
        if (backupRowEntity.Images.Any())
        {
            foreach (var image in backupRowEntity.Images)
            {
                backupImages.Add(new TierImageDto
                {
                    Id = image.Id,
                    Url = image.Url,
                    Note = image.Note,
                    ContainerId = image.ContainerId,
                    Order = image.Order,
                });
            }

            backupImages = backupImages.OrderBy(i => i.Order).ToList();
        }

        return new TierBackupRowDto
        {
            Id = backupRowEntity.Id,
            Images = backupImages.AsReadOnly(),
        };
    }
}
