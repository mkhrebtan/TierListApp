using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TierList.Application.Common.Settings;
using TierList.Domain.Entities;

namespace TierList.Application.Common.Services;

/// <summary>
/// Provides functionality for generating JSON Web Tokens (JWT) and refresh tokens for user authentication.
/// </summary>
/// <remarks>This service is responsible for creating access tokens and refresh tokens based on user information
/// and application-specific JWT settings. The generated tokens can be used for secure authentication and authorization
/// in a distributed system.</remarks>
public class TokenService : ITokenService
{
    private readonly IOptions<JwtSettings> _jwtSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenService"/> class with the specified JWT settings.
    /// </summary>
    /// <param name="jwtSettings">The JWT settings used to configure token generation and validation. This parameter cannot be null.</param>
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) access token for the specified user.
    /// </summary>
    /// <remarks>The generated token is signed using the HMAC-SHA256 algorithm and includes an expiration time
    /// based on the  configuration settings. Ensure that the <c>_jwtSettings</c> object is properly configured with
    /// valid values  for the issuer, audience, secret key, and token expiration duration.</remarks>
    /// <param name="user">The user for whom the access token is being generated. Must not be null.</param>
    /// <returns>A string representation of the generated JWT access token. The token includes claims such as the user's ID,
    /// username,  and other standard JWT claims, and is signed using the configured secret key.</returns>
    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.PreferredUsername, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Value.Issuer),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Value.Audience),
        };

        var key = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Value.Issuer,
            audience: _jwtSettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.Value.AccessTokenExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a new refresh token for the specified user.
    /// </summary>
    /// <remarks>The generated refresh token is a cryptographically secure random string encoded in Base64
    /// format.  The token is set to expire after a duration specified by the application's configuration
    /// settings.</remarks>
    /// <param name="user">The user for whom the refresh token is being generated. This parameter cannot be null.</param>
    /// <returns>A <see cref="RefreshToken"/> instance containing the generated token, its expiration time, and associated user
    /// information.</returns>
    public RefreshToken GenerateRefreshToken(User user)
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(randomBytes),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.Value.RefreshTokenExpirationDays),
            IsRevoked = false,
        };
    }
}
