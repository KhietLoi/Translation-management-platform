using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task<List<PasswordResetToken>> GetActiveTokensByUserIdAsync(Guid userId);
}