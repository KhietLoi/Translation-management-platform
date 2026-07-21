namespace MySolution.Application.Common.Interfaces.Authentication;

public interface ITokenSetting
{
    int EmailVerificationExpiryMinutes { get; }
    int PasswordResetExpiryMinutes { get; }
}