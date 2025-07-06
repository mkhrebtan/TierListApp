using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TierList.Application.Commands.TierImage;
using TierList.Application.Commands.TierList;
using TierList.Application.Commands.TierRow;
using TierList.Application.Common.DTOs.TierImage;
using TierList.Application.Common.DTOs.TierList;
using TierList.Application.Common.DTOs.TierRow;
using TierList.Application.Common.Enums;
using TierList.Application.Common.Interfaces;
using TierList.Application.Common.Models;
using TierList.Application.Queries;
using TierList.Domain.Abstraction;
using TierList.Domain.Entities;
using TierList.Domain.Repos;
using static System.Net.Mime.MediaTypeNames;

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
    private readonly IImageStorageService _imageStorageService;

    /// <summary>
    /// Initializes a new instance of the <see cref="TierListService"/> class.
    /// </summary>
    /// <param name="tierListRepository">The repository used to manage tier list data storage and retrieval.</param>
    /// <param name="unitOfWork">The unit of work instance used to manage transactional operations across multiple repositories.</param>
    /// <param name="imageStorageService">The service responsible for handling image storage and retrieval operations.</param>
    public TierListService(ITierListRepository tierListRepository, IUnitOfWork unitOfWork, IImageStorageService imageStorageService)
    {
        _tierListRepository = tierListRepository;
        _unitOfWork = unitOfWork;
        _imageStorageService = imageStorageService;
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
        else if (request.Title.Length > 100)
        {
            return TierListResult.Failure(
                error: "List title cannot exceed 100 characters.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.UserId <= 0)
        {
            return TierListResult.Failure(
                error: "Invalid user ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity tierList = new()
        {
            Title = request.Title,
            Created = DateTime.UtcNow,
            LastModified = DateTime.UtcNow,
            UserId = request.UserId,
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
        else if (request.UserId <= 0)
        {
            return TierListResult.Failure(
                error: "Invalid user ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listToDelete = await _tierListRepository.GetByIdAsync(request.Id);
        if (listToDelete is null)
        {
            return TierListResult.Failure(error: $"List with ID {request.Id} not found.", errorType: ErrorType.NotFound);
        }
        else if (listToDelete.UserId != request.UserId)
        {
            return TierListResult.Failure(
                error: $"List with ID {request.Id} does not belong to user with ID {request.UserId}.",
                errorType: ErrorType.ValidationError);
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            _tierListRepository.Delete(listToDelete);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
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
    public async Task<AllTierListsResult> GetTierListsAsync(GetTierListsQuery request)
    {
        if (request.UserId <= 0)
        {
            return AllTierListsResult.Failure(
                error: "Invalid user ID provided.",
                errorType: ErrorType.ValidationError);
        }

        var listsEntities = await _tierListRepository.GetAllAsync(request.UserId);
        var listsDtos = listsEntities
            .Select(l => new TierListBriefDto
            {
                Id = l.Id,
                Title = l.Title,
                Created = l.Created,
                LastModified = l.LastModified,
            }).ToList().AsReadOnly();

        return AllTierListsResult.Success(listsDtos);
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
        else if (request.UserId <= 0)
        {
            return TierListResult.Failure(
                error: "Invalid user ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.Title.Length > 100)
        {
            return TierListResult.Failure(
                error: "List title cannot exceed 100 characters.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? existingList = await _tierListRepository.GetByIdAsync(request.Id);
        if (existingList is null)
        {
            return TierListResult.Failure(error: $"List with ID {request.Id} not found.", errorType: ErrorType.NotFound);
        }
        else if (existingList.UserId != request.UserId)
        {
            return TierListResult.Failure(
                error: $"List with ID {request.Id} does not belong to user with ID {request.UserId}.",
                errorType: ErrorType.ValidationError);
        }

        existingList.Title = request.Title;
        existingList.LastModified = DateTime.UtcNow;

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
        else if (request.UserId <= 0)
        {
            return TierListResult.Failure(
                error: "Invalid user ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? tierList = await _tierListRepository.GetByIdAsync(request.Id);
        if (tierList is null)
        {
            return TierListResult.Failure(
                error: $"List with ID {request.Id} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (tierList.UserId != request.UserId)
        {
            return TierListResult.Failure(
                error: $"List with ID {request.Id} does not belong to user with ID {request.UserId}.",
                errorType: ErrorType.ValidationError);
        }

        TierBackupRowEntity? listBackupRowEntity = await _tierListRepository.GetBackupRowAsync(tierList.Id);
        if (listBackupRowEntity is null)
        {
            return TierListResult.Failure(
                error: $"Backup row for list with ID {request.Id} not found.",
                errorType: ErrorType.UnexpectedError);
        }

        IReadOnlyCollection<TierRowDto> listRows = await GetTierRowDtosAsync(tierList);
        TierBackupRowDto listBackupRowDto = await GetTierBackupRowDto(listBackupRowEntity);

        TierListDataDto tierListData = new()
        {
            Id = tierList.Id,
            Title = tierList.Title,
            Rows = listRows,
            BackupRow = listBackupRowDto,
        };

        return TierListResult.Success(tierListData);
    }

    public async Task<TierImageResult> GetImageUploadUrlAsync(GetTierImageUploadUrlQuery request)
    {
        if (string.IsNullOrEmpty(request.FileName) || string.IsNullOrEmpty(request.ContentType))
        {
            return TierImageResult.Failure("File name and content type cannot be empty.", ErrorType.ValidationError);
        }

        return await _imageStorageService.GetImageUploadUrlAsync(request.FileName, request.ContentType);
    }

    public async Task<TierImageResult> SaveImageTierImageAsync(SaveTierImageCommand request)
    {
        if (string.IsNullOrEmpty(request.Url))
        {
            return TierImageResult.Failure("Image URL cannot be empty.", ErrorType.ValidationError);
        }
        else if (request.StorageKey == Guid.Empty)
        {
            return TierImageResult.Failure("Storage key cannot be empty.", ErrorType.ValidationError);
        }
        else if (request.Order < 0)
        {
            return TierImageResult.Failure("Order must be a non-negative integer.", ErrorType.ValidationError);
        }
        else if (request.ContainerId <= 0)
        {
            return TierImageResult.Failure("Container ID must be greater than zero.", ErrorType.ValidationError);
        }

        TierImageContainer container = await _tierListRepository.GetBackupRowAsync(request.ListId);
        if (container is null)
        {
            return TierImageResult.Failure(
                error: $"Backup row for list with ID {request.ListId} not found.",
                errorType: ErrorType.UnexpectedError);
        }
        else if (container.Id != request.ContainerId)
        {
            return TierImageResult.Failure(
                error: $"Provided container with id {request.ContainerId} was not an id of backup row container",
                errorType: ErrorType.ValidationError);
        }

        IEnumerable<TierImageEntity> backupImages = await _tierListRepository.GetImagesAsync(container.Id);
        int imagesCount = backupImages.Count();
        if (request.Order > imagesCount + 1)
        {
            return TierImageResult.Failure(
                error: $"Order {request.Order} is out of range for the number of images in container {container.Id}.",
                errorType: ErrorType.ValidationError);
        }

        TierImageEntity imageEntity = new()
        {
            StorageKey = request.StorageKey,
            Url = request.Url,
            Note = request.Note,
            ContainerId = request.ContainerId,
            Order = request.Order,
        };

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.AddImage(imageEntity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.CreateTransactionAsync();
            return TierImageResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierImageResult.Success(
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

    public async Task<TierRowResult> CreateTierRowAsync(CreateTierRowCommand request)
    {
        if (request.ListId <= 0)
        {
            return TierRowResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (string.IsNullOrEmpty(request.Rank))
        {
            return TierRowResult.Failure(
                error: "Rank cannot be empty.",
                errorType: ErrorType.ValidationError);
        }
        else if (string.IsNullOrEmpty(request.ColorHex))
        {
            return TierRowResult.Failure(
                error: "Color cannot be empty.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.Order is not null && request.Order <= 0)
        {
            return TierRowResult.Failure(
                error: "Order must be a positive integer.",
                errorType: ErrorType.ValidationError);
        }

        Regex colorHexRegex = new(@"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$");
        if (!colorHexRegex.IsMatch(request.ColorHex))
        {
            return TierRowResult.Failure(
                error: "Invalid color format. Use #RRGGBB or #RGB.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierRowResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        int rowsCount = (await _tierListRepository.GetRowsAsync(request.ListId)).Count();
        if (request.Order is not null && request.Order > rowsCount + 1)
        {
            return TierRowResult.Failure(
                error: $"Order {request.Order} is out of range for the number of rows in list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        TierRowEntity newRow = new()
        {
            TierListId = request.ListId,
            Rank = request.Rank,
            ColorHex = request.ColorHex,
            Order = request.Order ?? rowsCount + 1,
        };
        try
        {
            await _unitOfWork.CreateTransactionAsync();
            _tierListRepository.AddRow(newRow);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TierRowResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierRowResult.Success(
            new TierRowBriefDto
            {
                Id = newRow.Id,
                Rank = newRow.Rank,
                ColorHex = newRow.ColorHex,
                Order = newRow.Order,
            });
    }

    public async Task<TierRowResult> UpdateTierRowAsync(IUpdateRowCommand request)
    {
        if (request.Id <= 0)
        {
            return TierRowResult.Failure(
                error: "Invalid row ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ListId <= 0)
        {
            return TierRowResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierRowResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        if (request is UpdateTierRowOrderCommand orderCommand)
        {
            IEnumerable<TierRowEntity> tierRowEntities = await _tierListRepository.GetRowsAsync(listEntity.Id);
            TierRowEntity? rowEntity = tierRowEntities.FirstOrDefault(r => r.Id == request.Id);
            if (rowEntity is null)
            {
                return TierRowResult.Failure(
                    error: $"Row with ID {request.Id} does not belong to list {request.ListId}.",
                    errorType: ErrorType.NotFound);
            }
            else if (orderCommand.Order > tierRowEntities.Count())
            {
                return TierRowResult.Failure(
                    error: $"Order {orderCommand.Order} is out of range for the number of rows in list {request.ListId}.",
                    errorType: ErrorType.ValidationError);
            }
            else if (orderCommand.Order == rowEntity.Order)
            {
                return TierRowResult.Success(
                    new TierRowBriefDto
                    {
                        Id = rowEntity.Id,
                        Rank = rowEntity.Rank,
                        ColorHex = rowEntity.ColorHex,
                        Order = rowEntity.Order,
                    });
            }

            var orderedRows = tierRowEntities.OrderBy(r => r.Order).ToList();
            orderedRows.Remove(rowEntity);
            orderedRows.Insert(orderCommand.Order - 1, rowEntity);
            for (int i = 0; i < orderedRows.Count; i++)
            {
                orderedRows[i].Order = i + 1;
            }

            try
            {
                await _unitOfWork.CreateTransactionAsync();

                foreach (var row in orderedRows)
                {
                    _tierListRepository.UpdateRow(row);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return TierRowResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
            }

            return TierRowResult.Success(
                new TierRowBriefDto
                {
                    Id = rowEntity.Id,
                    Rank = rowEntity.Rank,
                    ColorHex = rowEntity.ColorHex,
                    Order = rowEntity.Order,
                });
        }
        else
        {
            TierRowEntity? rowEntity = await _tierListRepository.GetRowByIdAsync(request.Id);
            if (rowEntity is null)
            {
                return TierRowResult.Failure(
                    error: $"Row with ID {request.Id} not found.",
                    errorType: ErrorType.NotFound);
            }

            if (request.ListId != rowEntity.TierListId)
            {
                return TierRowResult.Failure(
                    error: $"Row with ID {request.Id} does not belong to list {request.ListId}.",
                    errorType: ErrorType.ValidationError);
            }

            try
            {
                await _unitOfWork.CreateTransactionAsync();
                request.UpdateRow(rowEntity);
                _tierListRepository.UpdateRow(rowEntity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (ArgumentException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return TierRowResult.Failure(error: ex.Message, errorType: ErrorType.ValidationError);
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return TierRowResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
            }

            return TierRowResult.Success(
            new TierRowBriefDto
            {
                Id = rowEntity.Id,
                Rank = rowEntity.Rank,
                ColorHex = rowEntity.ColorHex,
                Order = rowEntity.Order,
            });
        }
    }

    public async Task<TierRowResult> DeleteTierRowAsync(DeleteTierRowCommand request)
    {
        if (request.Id <= 0)
        {
            return TierRowResult.Failure(
                error: "Invalid row ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ListId <= 0)
        {
            return TierRowResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierRowResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        TierRowEntity? rowEntity = await _tierListRepository.GetRowByIdAsync(request.Id);
        if (rowEntity is null)
        {
            return TierRowResult.Failure(
                error: $"Row with ID {request.Id} not found.",
                errorType: ErrorType.NotFound);
        }

        if (rowEntity.TierListId != request.ListId)
        {
            return TierRowResult.Failure(
                error: $"Row with ID {request.Id} does not belong to list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();

            if (!request.IsDeleteWithImages)
            {
                IEnumerable<TierImageEntity> rowImages = await _tierListRepository.GetImagesAsync(rowEntity.Id);
                if (rowImages.Any())
                {
                    TierBackupRowEntity? backupRowEntity = await _tierListRepository.GetBackupRowAsync(request.ListId);
                    if (backupRowEntity is null)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return TierRowResult.Failure(
                            error: $"Backup row for list with ID {request.ListId} not found.",
                            errorType: ErrorType.UnexpectedError);
                    }

                    int backupRowImagesCount = (await _tierListRepository.GetImagesAsync(backupRowEntity.Id)).Count();
                    foreach (var image in rowImages.OrderBy(i => i.Order).ToList())
                    {
                        image.ContainerId = backupRowEntity.Id;
                        image.Order = ++backupRowImagesCount;
                        _tierListRepository.UpdateImage(image);
                    }
                }
            }

            _tierListRepository.DeleteRow(rowEntity);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TierRowResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierRowResult.Success();
    }

    public async Task<TierImageResult> ReorderTierImageAsync(ReorderTierImageCommand request)
    {
        if (request.Id <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid image ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ListId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ContainerId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid container ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.Order < 0)
        {
            return TierImageResult.Failure(
                error: "Order must be a non-negative integer.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierImageResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        TierImageContainer? targetContainer = await _tierListRepository.GetContainerByIdAsync(request.ContainerId);
        if (targetContainer is null)
        {
            return TierImageResult.Failure(
                error: $"Container with ID {request.ContainerId} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (targetContainer.TierListId != request.ListId)
        {
            return TierImageResult.Failure(
                error: $"Container with ID {request.ContainerId} does not belong to list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        List<TierImageEntity> containerImages = (await _tierListRepository.GetImagesAsync(request.ContainerId)).OrderBy(i => i.Order).ToList();
        TierImageEntity? imageEntity = containerImages.FirstOrDefault(i => i.Id == request.Id);
        if (imageEntity is null)
        {
            return TierImageResult.Failure(
                error: $"Image with ID {request.Id} not found in container {request.ContainerId}.",
                errorType: ErrorType.ValidationError);
        }

        int imagesCount = containerImages.Count;
        int order = request.Order;
        if (order > imagesCount + 1)
        {
            return TierImageResult.Failure(
                error: $"Order {order} is out of range for the number of images in container {request.ContainerId}.",
                errorType: ErrorType.ValidationError);
        }
        else if (order == imageEntity.Order)
        {
            return TierImageResult.Success(
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
            return TierImageResult.Failure(
                error: ex.Message,
                errorType: ErrorType.SaveDataError);
        }

        return TierImageResult.Success(
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

    public async Task<TierImageResult> MoveTierImageAsync(MoveTierImageCommand request)
    {
        if (request.Id <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid image ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ListId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.FromContainerId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid source container ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ToContainerId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid target container ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.Order < 0)
        {
            return TierImageResult.Failure(
                error: "Order must be a non-negative integer.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierImageResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        TierImageContainer? sourceContainer = await _tierListRepository.GetContainerByIdAsync(request.FromContainerId);
        if (sourceContainer is null)
        {
            return TierImageResult.Failure(
                error: $"Source container with ID {request.ToContainerId} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (sourceContainer.TierListId != request.ListId)
        {
            return TierImageResult.Failure(
                error: $"Source container with ID {request.FromContainerId} does not belong to list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        TierImageContainer? targetContainer = await _tierListRepository.GetContainerByIdAsync(request.ToContainerId);
        if (targetContainer is null)
        {
            return TierImageResult.Failure(
                error: $"Target container with ID {request.ToContainerId} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (targetContainer.TierListId != request.ListId)
        {
            return TierImageResult.Failure(
                error: $"Target container with ID {request.ToContainerId} does not belong to list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        List<TierImageEntity> targetContainerImages = (await _tierListRepository.GetImagesAsync(request.ToContainerId)).OrderBy(i => i.Order).ToList();
        int imagesCount = targetContainerImages.Count;
        int order = request.Order;
        if (order > imagesCount + 1)
        {
            return TierImageResult.Failure(
                error: $"Order {order} is out of range for the number of images in target container {request.ToContainerId}.",
                errorType: ErrorType.ValidationError);
        }

        List<TierImageEntity> sourceContainerImages = (await _tierListRepository.GetImagesAsync(request.FromContainerId)).OrderBy(i => i.Order).ToList();
        TierImageEntity? imageEntity = sourceContainerImages.FirstOrDefault(i => i.Id == request.Id);
        if (imageEntity is null)
        {
            return TierImageResult.Failure(
                error: $"Image with ID {request.Id} not found in source container {request.FromContainerId}.",
                errorType: ErrorType.ValidationError);
        }

        sourceContainerImages.Remove(imageEntity);
        for (int i = 0; i < sourceContainerImages.Count; i++)
        {
            sourceContainerImages[i].Order = i + 1;
        }

        imageEntity.ContainerId = request.ToContainerId;
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
            return TierImageResult.Failure(
                error: ex.Message,
                errorType: ErrorType.SaveDataError);
        }

        return TierImageResult.Success(
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

    public async Task<TierImageResult> UpdateTierImageAsync(IUpdateImageCommand request)
    {
        if (request.Id <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid image ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ListId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ContainerId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid container ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierImageResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        TierImageContainer? containerEntity = await _tierListRepository.GetContainerByIdAsync(request.ContainerId);
        if (containerEntity is null)
        {
            return TierImageResult.Failure(
                error: $"Container with ID {request.ContainerId} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (containerEntity.TierListId != request.ListId)
        {
            return TierImageResult.Failure(
                error: $"Container with ID {request.ContainerId} does not belong to list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        TierImageEntity? imageEntity = await _tierListRepository.GetImageByIdAsync(request.Id);
        if (imageEntity is null)
        {
            return TierImageResult.Failure(
                error: $"Image with ID {request.Id} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (imageEntity.ContainerId != request.ContainerId)
        {
            return TierImageResult.Failure(
                error: $"Image with ID {request.Id} does not belong to container {request.ContainerId}.",
                errorType: ErrorType.ValidationError);
        }

        try
        {
            await _unitOfWork.CreateTransactionAsync();
            request.Update(imageEntity);
            _tierListRepository.UpdateImage(imageEntity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (ArgumentException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TierImageResult.Failure(error: ex.Message, errorType: ErrorType.ValidationError);
        }
        catch (InvalidOperationException ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return TierImageResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierImageResult.Success(
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

    public async Task<TierImageResult> DeleteTierImageAsync(DeleteTierImageCommand request)
    {
        if (request.Id <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid image ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ListId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid list ID provided.",
                errorType: ErrorType.ValidationError);
        }
        else if (request.ContainerId <= 0)
        {
            return TierImageResult.Failure(
                error: "Invalid container ID provided.",
                errorType: ErrorType.ValidationError);
        }

        TierListEntity? listEntity = await _tierListRepository.GetByIdAsync(request.ListId);
        if (listEntity is null)
        {
            return TierImageResult.Failure(
                error: $"List with ID {request.ListId} not found.",
                errorType: ErrorType.NotFound);
        }

        TierImageContainer? containerEntity = await _tierListRepository.GetContainerByIdAsync(request.ContainerId);
        if (containerEntity is null)
        {
            return TierImageResult.Failure(
                error: $"Container with ID {request.ContainerId} not found.",
                errorType: ErrorType.NotFound);
        }
        else if (containerEntity.TierListId != request.ListId)
        {
            return TierImageResult.Failure(
                error: $"Container with ID {request.ContainerId} does not belong to list {request.ListId}.",
                errorType: ErrorType.ValidationError);
        }

        List<TierImageEntity> containerImages = (await _tierListRepository.GetImagesAsync(request.ContainerId)).OrderBy(i => i.Order).ToList();
        TierImageEntity? imageEntity = containerImages.FirstOrDefault(i => i.Id == request.Id);
        if (imageEntity is null)
        {
            return TierImageResult.Failure(
                error: $"Image with ID {request.Id} does not belong to container {request.ContainerId}.",
                errorType: ErrorType.ValidationError);
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
            return TierImageResult.Failure(error: ex.Message, errorType: ErrorType.SaveDataError);
        }

        return TierImageResult.Success();
    }

    private async Task<IReadOnlyCollection<TierRowDto>> GetTierRowDtosAsync(TierListEntity listEntity)
    {
        IEnumerable<TierRowEntity> rowEntities = await _tierListRepository.GetRowsWithImagesAsync(listEntity.Id);
        List<TierRowDto> tierRowDtos = new();

        foreach (var rowEntity in rowEntities)
        {
            List<TierImageDto> rowImages = new();
            IEnumerable<TierImageEntity> rowImagesEntities = await _tierListRepository.GetImagesAsync(rowEntity.Id);
            if (rowImagesEntities.Any())
            {
                foreach (var image in rowEntity.Images)
                {
                    rowImages.Add(new TierImageDto
                    {
                        Id = image.Id,
                        StorageKey = image.StorageKey,
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

    private async Task<TierBackupRowDto> GetTierBackupRowDto(TierBackupRowEntity backupRowEntity)
    {
        List<TierImageDto> backupImages = new();
        IEnumerable<TierImageEntity> backupRowImages = await _tierListRepository.GetImagesAsync(backupRowEntity.Id);
        if (backupRowImages.Any())
        {
            foreach (var image in backupRowImages)
            {
                backupImages.Add(new TierImageDto
                {
                    Id = image.Id,
                    StorageKey = image.StorageKey,
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
