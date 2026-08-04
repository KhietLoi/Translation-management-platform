using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApiKeyRepository (AppDbContext context, ILogger logger) : Repository<ApiKey>(context, logger), IApiKeyRepository
{
    
}