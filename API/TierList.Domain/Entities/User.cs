using TierList.Domain.Abstraction;

namespace TierList.Domain.Entities;

/// <summary>
/// Represents a user in the system, including authentication and associated data.
/// </summary>
/// <remarks>This class contains properties for user identification, authentication, and related entities such as
/// tier lists and refresh tokens.</remarks>
public class User : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the username associated with the user.
    /// </summary>
    required public string Username { get; set; }

    /// <summary>
    /// Gets or sets the hashed representation of the user's password.
    /// </summary>
    required public string PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets the collection of tier lists associated with the entity.
    /// </summary>
    public ICollection<TierListEntity> TierLists { get; set; } = new List<TierListEntity>();

    /// <summary>
    /// Gets or sets the collection of refresh tokens associated with the user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}