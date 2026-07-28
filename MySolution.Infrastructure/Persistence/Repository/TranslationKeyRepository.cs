using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationKeyRepository (AppDbContext context, ILogger logger) : 
    Repository<TranslationKey>(context, logger), ITranslationKeyRepository
{
    public async Task<TranslationKey?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(x => x.Project)
            .Include(x => x.Namespace)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    

    public async Task<bool> ExistsAsync(Guid projectId, Guid namespaceId, string key, Guid? excludeKeyId = null)
    {
        return await DbSet.AnyAsync(x =>
            x.ProjectId == projectId &&
            x.NamespaceId == namespaceId &&
            x.Key == key &&
            (!excludeKeyId.HasValue || x.Id != excludeKeyId.Value));
    }
}