using TierList.Domain.Entities;

namespace TierList.Domain.Repos;

/// <summary>
/// Defines a repository interface for managing tier lists and their associated entities,  including rows and images,
/// with support for both asynchronous and queryable operations.
/// </summary>
/// <remarks>This interface provides methods for retrieving, adding, updating, and deleting tier list entities,
/// rows, and images. It supports both asynchronous operations for data retrieval and manipulation,  as well as
/// queryable operations for advanced querying scenarios.  Implementations of this interface are expected to handle
/// persistence and data access logic.</remarks>
public interface ITierListRepository : IRepository<TierListEntity>
{
    /// <summary>
    /// Retrieves all tier list entities associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose tier list entities are to be retrieved. Must be a positive integer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="TierListEntity"/> objects associated with the specified user. The list will be empty if no entities are
    /// found.</returns>
    Task<List<TierListEntity>> GetAllAsync(int userId);

    Task<TierListEntity?> GetTierListWithDataAsync(int id);
}