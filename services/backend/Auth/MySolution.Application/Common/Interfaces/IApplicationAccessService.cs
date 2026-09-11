namespace MySolution.Application.Common.Interfaces;

public interface IApplicationAccessService
{
    Task<bool> CanAccessProjectAsync (Guid projectId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Application?> GetApplicationAsync (CancellationToken cancellationToken = default);
}