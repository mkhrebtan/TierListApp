using Microsoft.EntityFrameworkCore;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Persistence.Postgres.Repos;

/// <summary>
/// Provides repository methods for managing tier lists, rows, and images in the database.
/// </summary>
/// <remarks>This repository extends <see cref="GenericRepository{TEntity}"/> to provide specialized operations
/// for tier lists, including retrieving, adding, updating, and deleting tier rows and images. It also supports both
/// asynchronous and queryable operations for flexible data access.</remarks>
public class TierListRepository : GenericRepository<TierListEntity>, ITierListRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TierListRepository"/> class with the specified database context.
    /// </summary>
    /// <param name="context">The <see cref="TierListDbContext"/> used to access the database. Cannot be null.</param>
    public TierListRepository(TierListDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Retrieves all tier lists associated with the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose tier lists are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see
    /// cref="TierListEntity"/> objects associated with the specified user. If no tier lists are found, the list will be
    /// empty.</returns>
    public async Task<List<TierListEntity>> GetAllAsync(int userId)
    {
        return await _context.TierLists
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TierListEntity?> GetTierListWithDataAsync(int id)
    {
        return await _context.TierLists
            .Include(t => t.Containers)
                .ThenInclude(r => r.Images)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
