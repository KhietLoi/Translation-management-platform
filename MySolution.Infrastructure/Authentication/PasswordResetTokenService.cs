using Microsoft.AspNetCore.DataProtection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Models;
using MySolution.Application.Constants;
using Newtonsoft.Json;

namespace MySolution.Infrastructure.Authentication;

public class PasswordResetTokenService : IPasswordResetTokenService
{
    private readonly IDataProtector _protector;

    public PasswordResetTokenService(IDataProtectionProvider protectionProvider)
    {
        _protector = protectionProvider.CreateProtector(nameof(PasswordResetTokenService));
    }

    public string GenerateResetToken(Guid userId, string email, string username, int passwordversion)
    {
        var payload = new PasswordResetPayload
            {
                UserId = userId,
                Email = email,
                PasswordVersion = passwordversion,
                ExpiredAt = DateTime.UtcNow.AddMinutes(AuthConstants.PasswordResetExpiryMinutes)
            };

        var json = JsonConvert.SerializeObject(payload);
        return _protector.Protect(json);
    }


    public PasswordResetPayload ValidateToken(string token)
    {
        var json = _protector.Unprotect(token);
        return JsonConvert.DeserializeObject<PasswordResetPayload>(json)!;
    }
}