using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces;

public interface IEmailVerificationTokenService
{
    string GenerateVerificationToken(Guid userid, string email);
    EmailVerifyPayload ValidateToken(string token);
}