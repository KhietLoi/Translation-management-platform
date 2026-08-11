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
}