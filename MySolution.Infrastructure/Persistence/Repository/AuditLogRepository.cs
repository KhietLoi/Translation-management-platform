using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class AuditLogRepository (AppDbContext context, ILogger logger) : 
    Repository<AuditLog>(context, logger), IAuditLogRepository
{
    public async Task<(List<AuditLog> Items, int TotalCount)> GetByEntityAsync(string entityName, Guid entityId, int pageNumber, int pageSize)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.EntityName == entityName && x.EntityId == entityId);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await query.CountAsync();

        return (items, totalCount); 
    }
}