using Microsoft.EntityFrameworkCore;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Seed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Permissions.AnyAsync())
        {
            return;
        }

        var permissions = new List<Permission>
        {
            new()
            {
                Id = SeedConstants.UserReadPermissionId,
                Description = "Read User",
                Code = "user.read"
            },
            new()
            {
                Id = SeedConstants.UserCreatePermissionId,
                Description = "Create User",
                Code = "user.create"
            },
            new()
            {
                Id = SeedConstants.UserUpdatePermissionId,
                Description = "Update User",
                Code = "user.update"
            },
            new()
            {
                Id = SeedConstants.UserDeletePermissionId,
                Description = "Delete User",
                Code = "user.delete"
            },
            new()
            {
                Id = SeedConstants.RoleReadPermissionId,
                Description = "Read Role",
                Code = "role.read"
            },
            new()
            {
                Id = SeedConstants.RoleCreatePermissionId,
                Description = "Create Role",
                Code = "role.create"
            },
            new()
            {
                Id = SeedConstants.RoleUpdatePermissionId,
                Description = "Update Role",
                Code = "role.update"
            },
            new()
            {
                Id = SeedConstants.RoleDeletePermissionId,
                Description = "Delete Role",
                Code = "role.delete"
            }
        };

        await context.Permissions.AddRangeAsync(permissions);

        await context.SaveChangesAsync();
    }
}