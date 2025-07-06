using TierList.Domain.Abstraction;
using TierList.Domain.Entities;

namespace TierList.Domain.Repos;

/// <summary>
/// Defines a repository for managing user entities, providing methods for retrieving and manipulating user data.
/// </summary>
/// <remarks>This interface extends <see cref="IRepository{T}"/> to include user-specific operations. It is
/// designed to handle user-related data access and retrieval, such as fetching users by unique identifiers or
/// usernames.</remarks>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Retrieves a user asynchronously based on the specified username.
    /// </summary>
    /// <param name="username">The username of the user to retrieve. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user object if found; 
    /// otherwise, <see langword="null"/>.</returns>
    Task<User?> GetByUsernameAsync(string username);

    void AddRefreshToken(RefreshToken token);

    Task<RefreshToken?> GetRefreshTokenAsync(string token);
}