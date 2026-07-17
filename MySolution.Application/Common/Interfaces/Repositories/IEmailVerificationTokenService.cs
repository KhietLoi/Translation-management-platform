using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IEmailVerificationTokenService
{
    //Generate Token:
    string GenerateVerificationToken(Guid userid,string email);
    EmailVerifyPayload ValidateToken(string token);
}