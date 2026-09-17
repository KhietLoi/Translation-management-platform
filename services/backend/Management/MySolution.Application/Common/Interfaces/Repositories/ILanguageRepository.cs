using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface ILanguageRepository : IRepository<Language>
{
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeProjectId = null);
}