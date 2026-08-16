using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationValueRepository (AppDbContext context, ILogger logger) : 
    Repository<TranslationValue>(context,logger), ITranslationValueRepository
{
    public async Task<TranslationValue?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.TranslationKey)
            .Include(x => x.Language)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TranslationValue>> GetAsync(
        Guid? translationKeyId,
        Guid? namespaceId,
        Guid? languageId,
        TranslationStatus? status)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.Language)
            .Include(x => x.TranslationKey)
            .ThenInclude(x => x.Namespace)
            .AsQueryable();

        if (translationKeyId.HasValue)
        {
            query = query.Where(x =>
                x.TranslationKeyId == translationKeyId.Value);
        }

        if (namespaceId.HasValue)
        {
            query = query.Where(x =>
                x.TranslationKey.NamespaceId == namespaceId.Value);
        }

        if (languageId.HasValue)
        {
            query = query.Where(x =>
                x.LanguageId == languageId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.Status == status.Value);
        }

        return await query
            .OrderBy(x => x.TranslationKey.Key)
            .ThenBy(x => x.Language.Code)
            .ToListAsync();
    }

    public async Task<TranslationValue?> GetByIdTrackingAsync(Guid id)
    {
        return await DbSet
            .Include(x => x.TranslationKey)
            .Include(x => x.Language)
            .Include(x => x.Reviewer)
            .FirstAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid translationKeyId, Guid languageId, Guid? excludeId = null)
    {
        return await DbSet.AnyAsync(x =>
            x.TranslationKeyId == translationKeyId &&
            x.LanguageId == languageId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var affectedRows = await DbSet
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }

    public async Task<Guid?> GetExistingIdAsync(Guid id)
    {
        return await DbSet
            .Where(x => x.Id == id)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(x => x.Id == id);
    }

    public async Task<List<TranslationValue>> GetReviewTranslationsAsync(Guid projectId, Guid languageId, Guid namespaceId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Include(x => x.TranslationKey)
            .Where(x =>
                x.TranslationKey.ProjectId == projectId &&
                x.TranslationKey.NamespaceId == namespaceId &&
                x.LanguageId == languageId &&
                x.Status == TranslationStatus.Translated)
            .OrderBy(x => x.TranslationKey.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TranslationValue>> GetForBatchReviewAsync(
        List<Guid> translationValueIds,
        Guid projectId,
        Guid languageId,
        Guid namespaceId)
    {
        return await DbSet
            .Include(x => x.TranslationKey)
            .ThenInclude(x => x.Project)
            .Include(x => x.TranslationKey)
            .ThenInclude(x => x.Namespace)
            .Where(x =>
                translationValueIds.Contains(x.Id) &&
                x.LanguageId == languageId &&
                x.TranslationKey.ProjectId == projectId &&
                x.TranslationKey.NamespaceId == namespaceId)
            .ToListAsync();
    }
}