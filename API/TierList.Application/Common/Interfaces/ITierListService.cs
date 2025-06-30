using TierList.Application.Commands.TierList;
using TierList.Application.Common.DTOs;
using TierList.Application.Common.Models;
using TierList.Application.Queries;

namespace TierList.Application.Common.Interfaces;

/// <summary>
/// Defines operations for managing tier lists, including creation, deletion, retrieval, and updates.
/// </summary>
/// <remarks>This service provides methods to handle tier list operations such as creating new tier lists,
/// retrieving existing tier lists, updating their details, and deleting them. Each method accepts  a specific command
/// or query object to encapsulate the required input data.</remarks>
public interface ITierListService
{
    /// <summary>
    /// Creates a tier list based on the specified request parameters.
    /// </summary>
    /// <param name="request">The command containing the parameters required to create the tier list.  This cannot be <see langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains  a <see cref="TierListResult"/>
    /// object representing the created tier list.</returns>
    Task<TierListResult> CreateTierListAsync(CreateTierListCommand request);

    /// <summary>
    /// Deletes a tier list based on the specified request.
    /// </summary>
    /// <remarks>Ensure that the <paramref name="request"/> contains valid data for identifying the tier list
    /// to delete. The operation may fail if the tier list does not exist or if the user lacks the necessary
    /// permissions.</remarks>
    /// <param name="request">The command containing the details of the tier list to delete.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.  The task result contains a <see
    /// cref="TierListResult"/> indicating the outcome of the deletion operation.</returns>
    Task<TierListResult> DeleteTierListAsync(DeleteTierListCommand request);

    /// <summary>
    /// Retrieves a collection of tier lists based on the specified query parameters.
    /// </summary>
    /// <param name="request">The query parameters used to filter and retrieve the tier lists. This parameter cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only collection of  <see
    /// cref="TierListBriefDto"/> objects that match the query parameters. If no tier lists are found, the collection
    /// will be empty.</returns>
    Task<IReadOnlyCollection<TierListBriefDto>> GetTierListsAsync(GetTierListsQuery request);

    /// <summary>
    /// Updates the tier list based on the provided command.
    /// </summary>
    /// <remarks>This method performs an asynchronous operation to update the tier list. Ensure that the
    /// <paramref name="request"/> parameter contains valid data to avoid errors during the update process.</remarks>
    /// <param name="request">The command containing the details required to update the tier list.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="TierListResult"/>
    /// object with the outcome of the update operation.</returns>
    Task<TierListResult> UpdateTierListAsync(UpdateTierListCommand request);

    /// <summary>
    /// Retrieves tier list data based on the specified query parameters.
    /// </summary>
    /// <remarks>Use this method to asynchronously fetch tier list data based on the provided query.  Ensure
    /// that the <paramref name="request"/> parameter is properly populated with valid query criteria.</remarks>
    /// <param name="request">The query parameters used to filter and retrieve the tier list data.  This parameter cannot be <see
    /// langword="null"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="TierListResult"/>
    /// object with the retrieved tier list data.</returns>
    Task<TierListResult> GetTierListDataAsync(GetTierListDataQuery request);

    Task<TierImageResult> GetImageUploadUrlAsync(GetTierImageUploadUrlQuery request);

    Task<TierImageResult> GetImageDownloadUrlAsync(GetTierImageDownloadUrlQuery request);
}
