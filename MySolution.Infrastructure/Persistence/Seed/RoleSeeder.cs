using Microsoft.EntityFrameworkCore;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Roles.AnyAsync())
        {
            return;
        }

        var roles = new List<Role>
        {
            new()
            {
                Id = SeedConstants.AdminRoleId,
                Name = "Admin"
            },
            new()
            {
                Id = SeedConstants.UserRoleId,
                Name = "User"
            }
        };

        await context.Roles.AddRangeAsync(roles);

        await context.SaveChangesAsync();
    }
}