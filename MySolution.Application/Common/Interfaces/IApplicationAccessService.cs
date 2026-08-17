namespace MySolution.Application.Common.Interfaces;

public interface IApplicationAccessService
{
    Task<bool> CanAccessProjectAsync (Guid projectId, CancellationToken cancellationToken = default);
}