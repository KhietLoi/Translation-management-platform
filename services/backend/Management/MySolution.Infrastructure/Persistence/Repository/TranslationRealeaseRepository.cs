using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class TranslationReleaseRepository (AppDbContext context, ILogger logger)
    : Repository<TranslationRelease>(context, logger), ITranslationReleaseRepository
{
    public async Task<TranslationRelease?> GetByIdAsync(Guid releaseId, CancellationToken cancellationToken)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == releaseId, cancellationToken);
    }

    // public async Task<TranslationRelease?> GetCurrentActiveAsync(Guid projectId, CancellationToken cancellationToken)
    // {
    //     return await DbSet.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.IsActive, cancellationToken);
    // }

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

    // public async Task<(List<ReleaseHistoryItemDto> Items, int TotalCount)> GetReleaseHistoryAsync(Guid projectId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    // {
    //     var query =
    //         from release in DbSet.AsNoTracking()
    //         join user in Context.Users.AsNoTracking()
    //             on release.PublishedBy equals user.Id
    //         where release.ProjectId == projectId
    //         select new ReleaseHistoryItemDto
    //         {
    //             Id = release.Id,
    //             ProjectId = release.ProjectId,
    //             BlobFileName =  release.BlobFileName,
    //             DownloadUrl = release.DownloadUrl,
    //             Version = release.Version,
    //             PublishedAt = release.PublishedAt,
    //             PublishedBy = release.PublishedBy,
    //             PublishedByName = user.Username,
    //             Notes = release.Notes,
    //             IsActive = release.IsActive,
    //             TotalKey = release.TotalKey
    //         };
    //
    //     var totalCount = await query.CountAsync(cancellationToken);
    //
    //     var items = await query
    //         .OrderByDescending(x => x.Version)
    //         .Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync(cancellationToken);
    //
    //     return (items, totalCount);
    // }

    // public async Task<TranslationRelease?> GetPreviousReleaseAsync(Guid projectId, DateTime currentReleasePublishedAt, CancellationToken cancellationToken)
    // {
    //     return await DbSet
    //         .Where(x =>
    //             x.ProjectId == projectId &&
    //             x.PublishedAt < currentReleasePublishedAt)
    //         .OrderByDescending(x => x.PublishedAt)
    //         .FirstOrDefaultAsync(cancellationToken);
    // }

    public async Task<TranslationRelease?> GetActiveReleaseAsync(Guid projectId, CancellationToken cancellationToken)
    {
        return await DbSet
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.IsActive, cancellationToken);
    }
}   