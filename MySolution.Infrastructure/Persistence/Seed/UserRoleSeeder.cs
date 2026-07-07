using Microsoft.EntityFrameworkCore;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Seed;

public static class UserRoleSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.UserRoles.AnyAsync())
        {
            return;
        }

        var userRole = new UserRole
        {
            UserId = SeedConstants.AdminUserId,
            RoleId = SeedConstants.AdminRoleId
        };

        await context.UserRoles.AddAsync(userRole);

        await context.SaveChangesAsync();
    }
}