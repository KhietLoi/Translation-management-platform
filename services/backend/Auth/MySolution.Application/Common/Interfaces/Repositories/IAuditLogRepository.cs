using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    //Task AddAsync(AuditLog auditLog);
    Task<(List<AuditLog> Items, int TotalCount)> GetByEntityAsync(string entityName, Guid entityId, int pageNumber, int pageSize);
    Task<List<AuditLog>> GetRecentActivitiesAsync(IReadOnlyCollection<Guid> projectIds, int limit, CancellationToken cancellationToken);
}