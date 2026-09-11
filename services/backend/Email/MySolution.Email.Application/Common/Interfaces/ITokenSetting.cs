namespace MySolution.Email.Application.Common.Interfaces;

public interface ITokenSetting
{
    int EmailVerificationExpiryMinutes { get; }
    int PasswordResetExpiryMinutes { get; }
}