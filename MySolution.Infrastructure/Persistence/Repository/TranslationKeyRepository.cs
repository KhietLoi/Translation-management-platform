using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

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
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(x => x.Id == id);
    }

    public async Task<Guid> GetProjectIdAsync(Guid translationKeyId)
    {
        return await DbSet
            .Where(x => x.Id == translationKeyId)
            .Select(x => x.ProjectId)
            .FirstOrDefaultAsync();
    }

    public async Task <(List<TranslationKey> Items, int TotalCount)> GetByGridAsync
    (
        Guid? projectId,
        Guid? namespaceId,
        string? keyword,
        TranslationStatus? status,
        int pageNumber, int pageSize
    )
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.Namespace)
            .Include(x => x.TranslationValues)
            .ThenInclude(x => x.Language)
            .Where(x => x.ProjectId == projectId);

        if (namespaceId.HasValue)
        {
            query = query.Where(x =>
                x.NamespaceId == namespaceId.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x =>
                x.Key.Contains(keyword) ||
                (x.Description != null &&
                 x.Description.Contains(keyword)));
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.TranslationValues.Any(v =>
                    v.Status == status.Value));
        }

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(x => x.Key)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }
}