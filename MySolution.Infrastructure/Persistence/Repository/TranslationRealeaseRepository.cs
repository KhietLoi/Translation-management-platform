using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationReleaseRepository (AppDbContext context, ILogger logger)
    : Repository<TranslationRelease>(context, logger), ITranslationReleaseRepository
{
    public async Task<int> GetLatestVersionAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await DbSet
                   .Where(x => x.ProjectId == projectId)
                   .MaxAsync(x => (int?)x.Version,
                       cancellationToken)
               ?? 0;
    }

    public async Task<List<TranslationRelease>> GetActiveByProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await DbSet
            .Where(x => x.ProjectId == projectId)
            .Where(x => x.IsActive == true)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistReleaseWithChecksumAsync(Guid projectId, string checksum, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(x => x.ProjectId == projectId && x.Checksum == checksum, cancellationToken);
    }

    public async Task<(List<TranslationRelease> Items, int TotalCount)> GetReleaseHistoryAsync(Guid projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = DbSet.AsNoTracking().Where(x => x.ProjectId == projectId);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.Version)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}