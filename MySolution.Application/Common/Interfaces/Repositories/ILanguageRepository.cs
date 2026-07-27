using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ILanguageRepository : IRepository<Language>
{
    Task<Language?> GetByIdAsync(Guid id);
    Task<Language?> GetByCodeAsync(string code);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeProjectId = null);
}