using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IUserProfileRepository : IRepository<UserProfile>
{
    Task<UserProfile?> GetByIdAsync(Guid userId);
    Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, Guid? excludeUserId = null);
}