namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IApplicationRepository : IRepository<Domain.Entities.Application>
{
    Task <Domain.Entities.Application?> GetByIdAsync(Guid applicationId, CancellationToken cancellationToken);
    Task <bool> IsApplicationNameExistsAsync(string name, Guid? excludeId = null);
}