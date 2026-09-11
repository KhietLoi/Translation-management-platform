using Microsoft.AspNetCore.DataProtection;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Models;
using Newtonsoft.Json;

namespace MySolution.Infrastructure.Authentication;

public class PasswordResetTokenService : IPasswordResetTokenService
{
    private readonly IDataProtector _protector;
    private readonly ITokenSetting _tokenSetting;

    public PasswordResetTokenService(IDataProtectionProvider protectionProvider, ITokenSetting tokenSetting)
    {
        _protector = protectionProvider.CreateProtector(nameof(PasswordResetTokenService));
        _tokenSetting = tokenSetting;
    }

    public string GenerateResetToken(Guid userId, string email, string username, int passwordversion)
    {
        var payload = new PasswordResetPayload
        {
            UserId = userId,
            Email = email,
            PasswordVersion = passwordversion,
            ExpiredAt = DateTime.UtcNow.AddMinutes(_tokenSetting.PasswordResetExpiryMinutes)
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