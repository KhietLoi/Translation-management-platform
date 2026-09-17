using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsByEmailOrUsernameAsync(string email, string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}