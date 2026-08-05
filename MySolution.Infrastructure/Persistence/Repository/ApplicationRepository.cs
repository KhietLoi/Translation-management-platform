using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApplicationRepository (AppDbContext context, ILogger logger) : Repository<Domain.Entities.Application>(context, logger), IApplicationRepository
{
    public async Task<Domain.Entities.Application?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<bool> IsApplicationNameExistsAsync(string name, Guid? excludeId = null)
    {
        return await DbSet.AnyAsync
            (a => a.Name == name && 
                  (!excludeId.HasValue || a.Id != excludeId.Value));
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(a => a.Id == id);
    }
}