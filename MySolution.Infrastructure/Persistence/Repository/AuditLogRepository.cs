using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class AuditLogRepository (AppDbContext context, ILogger logger) : 
    Repository<AuditLog>(context, logger), IAuditLogRepository
{
    
}