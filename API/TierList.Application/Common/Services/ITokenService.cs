using TierList.Domain.Entities;

namespace TierList.Application.Common.Services;

/// <summary>
/// Defines methods for generating access and refresh tokens for user authentication.
/// </summary>
/// <remarks>This interface provides functionality for creating tokens used in authentication and authorization
/// workflows. Implementations of this interface should ensure that tokens are securely generated and meet the
/// application's requirements.</remarks>
public interface ITokenService
{
    /// <summary>
    /// Generates a new access token for the specified user.
    /// </summary>
    /// <param name="user">The user for whom the access token is generated. This parameter cannot be null.</param>
    /// <returns>A string representing the generated access token. The token is unique to the user and can be used for
    /// authentication purposes.</returns>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generates a new refresh token for the specified user.
    /// </summary>
    /// <remarks>The generated refresh token is unique and can be used to obtain a new access token for the
    /// specified user. Ensure that the user object provided is valid and corresponds to an existing user in the
    /// system.</remarks>
    /// <param name="user">The user for whom the refresh token is being generated. This parameter cannot be null.</param>
    /// <returns>A <see cref="RefreshToken"/> object containing the newly generated token and associated metadata.</returns>
    RefreshToken GenerateRefreshToken(User user);
}