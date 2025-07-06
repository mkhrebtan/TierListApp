using Microsoft.EntityFrameworkCore;
using TierList.Domain.Entities;
using TierList.Domain.Repos;

namespace TierList.Persistence.Postgres.Repos;

/// <summary>
/// Provides methods for managing user entities in a data store.
/// </summary>
/// <remarks>This repository interface defines operations for retrieving, inserting, updating,  and deleting user
/// entities. It supports asynchronous retrieval of users by ID or  a combination of ID and username. Implementations of
/// this class are expected to  handle the persistence and retrieval of user data.</remarks>
public class UserRepository : GenericRepository<User>,  IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class with the specified database context.
    /// </summary>
    /// <param name="context">The <see cref="TierListDbContext"/> used to access the database. Cannot be null.</param>
    public UserRepository(TierListDbContext context)
        : base(context)
    {
    }

    /// <summary>
    /// Retrieves a user that matches the specified username.
    /// </summary>
    /// <remarks>This method performs a case-sensitive search for the username. If no matching user is found,
    /// the method returns <see langword="null"/>.</remarks>
    /// <param name="username">The username of the user to retrieve. This comparison is case-sensitive.</param>
    /// <returns>A <see cref="User"/> object if a user with the specified username is found; otherwise, <see
    /// langword="null"/>.</returns>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public void AddRefreshToken(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
    }
}
