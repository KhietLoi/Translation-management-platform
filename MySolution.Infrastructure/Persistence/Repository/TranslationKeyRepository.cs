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
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<Guid?> GetExistingIdAsync(Guid id)  // => id TranslationKey
    {
        return await DbSet
            .Where(x => x.Id == id)
            .Select(x=>(Guid?)x.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<TranslationKey?> GetByIdTrackingAsync(Guid id)
    {
        return await DbSet
            .Include(x => x.Project)
            .Include(x => x.Namespace)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TranslationKey>> GetAsync(Guid? projectId, Guid? namespaceId, string? keyword)
    {
        var query =  DbSet
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Namespace)
            .AsQueryable();
        var count = await query.CountAsync();
        Console.WriteLine(count);
        
        if (projectId.HasValue)
        {
            query = query.Where(x => x.ProjectId == projectId.Value);
        }

        if (namespaceId.HasValue)
        {
            query = query.Where(x => x.NamespaceId == namespaceId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => EF.Functions.ILike(x.Key, $"%{keyword}%"));
        }

        return await query
            .OrderBy(x => x.Key)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid projectId, Guid namespaceId, string key, Guid? excludeKeyId = null)
    {
        return await DbSet.AnyAsync(x =>
            x.ProjectId == projectId &&
            x.NamespaceId == namespaceId &&
            x.Key == key &&
            (!excludeKeyId.HasValue || x.Id != excludeKeyId.Value));
    }

    /*public async Task<bool> IsNamespaceBelongsToProjectAsync(Guid namespaceId, Guid projectId)
    {
        return await DbSet.AnyAsync(x => x.NamespaceId == namespaceId && x.ProjectId == projectId);
    }*/
}