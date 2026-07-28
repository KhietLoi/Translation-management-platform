using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectMemberRepository (AppDbContext context, ILogger logger)
    : Repository<ProjectMember>(context, logger), IProjectMemberRepository
{
    public async Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId)
    {
        return await DbSet
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<List<ProjectMember>> GetByProjectIdWithUserAsync(Guid projectId)
    {
        return await DbSet
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<List<ProjectMember>> GetByProjectIdWithUserTrackingAsync(Guid projectId)
    {
        return await DbSet
            .Include(x => x.User)
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();
    }
}