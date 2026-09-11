using MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;
using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IApiKeyRepository : IRepository<ApiKey>
{
    Task <ApiKey?> GetByIdAsync(Guid id);
    Task<bool> IsApiKeyExistsAsync(Guid applicationId, string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<ApiKey?> GetByIdWithPermissionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<GetApiKeyGridData> Items, int TotalItems)>
        GetGridAsync
        (
            Guid? projectId,
            Guid? applicationId,
            string? keyword,
            bool? isRevoked,
            int page,
            int limit,
            CancellationToken cancellationToken
        );
    Task<ApiKey?> GetByHashAsync(string hash, CancellationToken cancellationToken);
}
