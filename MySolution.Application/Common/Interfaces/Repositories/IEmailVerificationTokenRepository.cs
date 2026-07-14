using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository :  IRepository<EmailVerificationToken>
{
    //Task Add(EmailVerificationToken token);

    Task<EmailVerificationToken?> GetByTokenAsync(string token);

    Task<List<EmailVerificationToken>> GetActiveTokensByUserId(Guid userId);
}