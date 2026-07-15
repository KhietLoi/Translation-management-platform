namespace MySolution.Infrastructure.Authentication;

public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public int ExpireMinutes { get; set; } //For AccessToken
    public int RefreshTokenDays { get; set; } //For RefreshToken
}