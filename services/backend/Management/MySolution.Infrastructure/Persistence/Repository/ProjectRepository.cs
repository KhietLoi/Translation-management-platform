using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ProjectRepository (AppDbContext context, ILogger logger) : Repository<Project>(context, logger), IProjectRepository
{
    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeProjectId = null )
    {
        return await DbSet.AnyAsync
            (
                x  => x.Name == name && (!excludeProjectId.HasValue || x.Id != excludeProjectId.Value)
            );
    }
}