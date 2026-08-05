using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IApiKeyRepository : IRepository<ApiKey>
{
    Task <ApiKey?> GetByIdAsync(Guid id);
    Task<bool> IsApiKeyExistsAsync(Guid applicationId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<ApiKey?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
}