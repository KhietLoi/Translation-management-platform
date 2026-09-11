using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApiKeyUsageLogRepository (AppDbContext context, ILogger logger) : Repository<ApiKeyUsageLog>(context, logger), IApiKeyUsageLogRepository
{
    
}