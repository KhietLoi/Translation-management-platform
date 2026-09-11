using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class LanguageRepository(AppDbContext context, ILogger logger)
    : Repository<Language>(context, logger), ILanguageRepository
{
    
    public async Task<Language?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<Language?> GetByCodeAsync(string code)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeProjectId = null)
    {
        return await DbSet.AnyAsync
        (
            x  => x.Code == code &&
                  (!excludeProjectId.HasValue || x.Id != excludeProjectId.Value)
        );
    }

    public async Task<List<Language>> GetByIdsAsync(List<Guid> ids)
    {
        return await DbSet.Where(x => ids.Contains(x.Id)).ToListAsync();
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await DbSet.AnyAsync(x => x.Id == id);
    }
    
}