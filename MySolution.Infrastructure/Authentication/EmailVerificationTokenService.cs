using Microsoft.AspNetCore.DataProtection;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Application.Constants;
using Newtonsoft.Json;

namespace MySolution.Infrastructure.Authentication;

public class EmailVerificationTokenService : IEmailVerificationTokenService
{
    private readonly IDataProtector _protector;

    public EmailVerificationTokenService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(nameof(EmailVerifyPayload));
    }
    public string GenerateVerificationToken(Guid userid,string email)
    {
        var payload = new EmailVerifyPayload
        {
            UserId = userid,
            Email = email,
            ExpiredAt = DateTime.UtcNow.AddMinutes(AuthConstants.PasswordResetExpiryMinutes)
        };
        var json = JsonConvert.SerializeObject(payload);
        //Decrypt:
        return _protector.Protect(json);
    }

    public EmailVerifyPayload ValidateToken(string token)
    {
        var json = _protector.Unprotect(token);
        var payload = JsonConvert.DeserializeObject<EmailVerifyPayload>(json);
        if (payload == null)
        {
            throw new InvalidOperationException("Invalid verification token.");
        }
        
        return payload;
    }
}