namespace MySolution.Infrastructure.Options;

public class TokenOptions
{
    public const string SectionName = "TokenOptions";
    public int EmailVerificationTokenExpireMinutes { get; set; }
    public int PasswordResetTokenExpireMinutes { get; set; }
}