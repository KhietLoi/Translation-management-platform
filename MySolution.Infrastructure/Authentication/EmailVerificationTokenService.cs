using Microsoft.AspNetCore.DataProtection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Models;
using Newtonsoft.Json;

namespace MySolution.Infrastructure.Authentication;

public class EmailVerificationTokenService : IEmailVerificationTokenService
{
    private readonly IDataProtector _protector;
    private readonly ITokenSetting _tokenSetting;

    public EmailVerificationTokenService(IDataProtectionProvider dataProtectionProvider, ITokenSetting tokenSetting)
    {
        _protector = dataProtectionProvider.CreateProtector(nameof(EmailVerifyPayload));
        _tokenSetting = tokenSetting;
    }

    public string GenerateVerificationToken(Guid userid, string email)
    {
        var payload = new EmailVerifyPayload
        {
            UserId = userid,
            Email = email,
            ExpiredAt= DateTime.UtcNow.AddMinutes(_tokenSetting.EmailVerificationExpiryMinutes)
        };
        var json = JsonConvert.SerializeObject(payload);
        //Decrypt:
        return _protector.Protect(json);
    }

    public EmailVerifyPayload ValidateToken(string token)
    {
        var json = _protector.Unprotect(token);
        var payload = JsonConvert.DeserializeObject<EmailVerifyPayload>(json);
        if (payload == null) throw new InvalidOperationException("Invalid verification token.");

        return payload;
    }
}