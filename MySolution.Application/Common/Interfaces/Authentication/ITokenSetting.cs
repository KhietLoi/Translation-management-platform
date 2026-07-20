namespace MySolution.Application.Common.Interfaces;

public interface ITokenSetting
{
    int EmailVerificationExpiryMinutes { get; set; }
    int PasswordResetExpiryMinutes { get; set; }
}