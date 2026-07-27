using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectRepository (AppDbContext context, ILogger logger)
    : Repository<Project>(context, logger), IProjectRepository
{
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<Project?> GetByNameAsync(string name)
    {
        return await DbSet.FirstOrDefaultAsync(x  => x.Name == name);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeProjectId = null )
    {
        return await DbSet.AnyAsync
            (
                x  => x.Name == name &&
                (!excludeProjectId.HasValue || x.Id != excludeProjectId.Value)
            );
    }

    public async Task<Project?> GetDetailAsync(Guid id)
    {
        return await Context.Projects
            .AsSplitQuery()
            .Include(x =>x.ProjectLanguages)
                .ThenInclude(x => x.Language)
            .Include(x=>x.ProjectMembers)
                .ThenInclude(x=>x.User)
            .Include(x => x.ProjectNamespaces)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}