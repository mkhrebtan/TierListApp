namespace TierList.Application.Common.Settings;

public class JwtSettings
{
    required public string SecretKey { get; set; }

    required public string Issuer { get; set; }

    required public string Audience { get; set; }

    required public int AccessTokenExpirationMinutes { get; set; }

    required public int RefreshTokenExpirationDays { get; set; }
}
