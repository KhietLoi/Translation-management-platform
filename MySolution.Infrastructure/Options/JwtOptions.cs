using System.ComponentModel.DataAnnotations;

namespace MySolution.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    public int ExpireMinutes { get; set; }

    public int RefreshTokenDays { get; set; }
}