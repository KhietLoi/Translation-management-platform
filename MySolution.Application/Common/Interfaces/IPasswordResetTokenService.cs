using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces;

public interface IPasswordResetTokenService
{
    //Generate:
    string GenerateResetToken(Guid userId, string email, string username, int passwordversion);
    //Validate
    PasswordResetPayload ValidateToken(string token);
}