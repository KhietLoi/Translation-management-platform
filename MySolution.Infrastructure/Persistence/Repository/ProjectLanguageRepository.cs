using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectLanguageRepository (AppDbContext context, ILogger logger)
    : Repository<ProjectLanguage>(context, logger), IProjectLanguageRepository
{

    public async Task<List<ProjectLanguage>> GetByProjectIdAsync(Guid projectId)
    {
        return await DbSet
            .Where(p => p.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<List<ProjectLanguage>> GetByProjectIdsAsync(List<Guid> projectIds)
    {
        return await DbSet
            .Where(p => projectIds.Contains(p.ProjectId))
            .ToListAsync();
    }

    public async Task<List<ProjectLanguage>> GetByProjectIdWithLanguageAsync(Guid projectId)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.Language)
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<bool> IsLanguageBelongsToProjectAsync(Guid languageId, Guid projectId)
    {
        return await DbSet.AnyAsync(x => x.LanguageId == languageId && x.ProjectId == projectId);
    }
}