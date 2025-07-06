namespace TierList.Domain.Entities;

/// <summary>
/// Represents a refresh token used for authentication and session management.
/// </summary>
/// <remarks>A refresh token is typically issued to a user upon successful authentication and is used to obtain
/// new access tokens without requiring the user to reauthenticate. This class contains information about the token, its
/// expiration, and its association with a user.</remarks>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    required public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the token used for authentication or authorization purposes.
    /// </summary>
    required public string Token { get; set; }

    /// <summary>
    /// Gets or sets the expiration date and time for the associated entity.
    /// </summary>
    required public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// Gets or sets the user associated with the current context.
    /// </summary>
    public User? User { get; set; }
}
