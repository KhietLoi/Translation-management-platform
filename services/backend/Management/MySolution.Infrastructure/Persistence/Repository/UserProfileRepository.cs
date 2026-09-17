using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Repository;

public class UserProfileRepository (AppDbContext context, ILogger logger) : Repository<UserProfile>(context, logger), IUserProfileRepository
{
    public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber, Guid? excludeUserId = null)
    {
        var query = DbSet.Where(x => x.PhoneNumber == phoneNumber);
        if (excludeUserId.HasValue)
        {
            query = query.Where(x => x.UserId != excludeUserId.Value);
        }
        return await query.AnyAsync();
    }
}