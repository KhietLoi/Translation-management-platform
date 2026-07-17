namespace MySolution.Application.Common.Interfaces;

public interface IApplicationUrlProvider
{
    string GetVerifyEmailUrl(string token);
    string GetResetPasswordUrl(string token);
}