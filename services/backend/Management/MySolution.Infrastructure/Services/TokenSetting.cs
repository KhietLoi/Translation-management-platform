using Microsoft.Extensions.Options;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Infrastructure.Options;

namespace MySolution.Infrastructure.Services;

public class TokenSetting : ITokenSetting
{
    private readonly TokenOptions _tokenOptions;

    public TokenSetting(IOptions<TokenOptions> tokenOptions)
    {
        _tokenOptions = tokenOptions.Value;
    }

    public int EmailVerificationExpiryMinutes => _tokenOptions.EmailVerificationTokenExpireMinutes;

    public int PasswordResetExpiryMinutes => _tokenOptions.PasswordResetTokenExpireMinutes;
}