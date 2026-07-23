using Microsoft.Extensions.Options;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Infrastructure.Options;

namespace MySolution.Email.Infrastructure.Services;

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