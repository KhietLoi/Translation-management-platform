using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApplicationRepository (AppDbContext context, ILogger logger) : Repository<Domain.Entities.Application>(context, logger), IApplicationRepository
{
    public async Task<Domain.Entities.Application?> GetByIdAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        return await DbSet.FindAsync(applicationId, cancellationToken);
    }

    // public async Task<bool> IsApplicationNameExistsAsync(string name, Guid? excludeId = null)
    // {
    //     return await DbSet.AnyAsync(a => a.Name == name && (!excludeId.HasValue || a.Id != excludeId.Value));
    // }
}