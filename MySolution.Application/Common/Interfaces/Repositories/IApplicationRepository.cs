namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IApplicationRepository : IRepository<Domain.Entities.Application>
{
    Task <Domain.Entities.Application?> GetByIdAsync(Guid id);
    Task <bool> IsApplicationNameExistsAsync(string name, Guid? excludeId = null);
    Task<bool> ExistsAsync(Guid id);
}