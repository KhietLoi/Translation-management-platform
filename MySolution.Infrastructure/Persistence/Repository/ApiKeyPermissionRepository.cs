using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class ApiKeyPermissionRepository (AppDbContext context, ILogger logger) : Repository<ApiKeyPermission>(context,logger), IApiKeyPermissionRepository
{
}