using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IApiKeyPermissionRepository : IRepository<ApiKeyPermission>
{
    Task<List<ApiKeyPermission>?> GetPermissionsByApiKeyId(Guid apiKeyId);
}