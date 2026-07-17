using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.Services;

public class ApplicationUrlProvider : IApplicationUrlProvider
{
    private readonly FrontendOptions  _frontendOptions;

    public ApplicationUrlProvider(IOptions<FrontendOptions> frontendOptions)
    {
        _frontendOptions = frontendOptions.Value;
    }
    public string GetVerifyEmailUrl(string token)
    {
        return $"{_frontendOptions.BaseUrl}/verify-email?token={token}";
    }

    public string GetResetPasswordUrl(string resetToken)
    {
        return $"{_frontendOptions.BaseUrl}/reset-password?token={resetToken}";
    }
}