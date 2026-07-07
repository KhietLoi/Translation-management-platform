using Microsoft.EntityFrameworkCore;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Seed;

public static class RolePermissionSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.RolePermissions.AnyAsync())
        {
            return;
        }

        var permissions = await context.Permissions
            .Select(x => x.Id)
            .ToListAsync();

        var rolePermissions = permissions
            .Select(permissionId => new RolePermission
            {
                RoleId = SeedConstants.AdminRoleId,
                PermissionId = permissionId
            })
            .ToList();

        // User role only has Read User permission
        rolePermissions.Add(new RolePermission
        {
            RoleId = SeedConstants.UserRoleId,
            PermissionId = SeedConstants.UserReadPermissionId
        });

        await context.RolePermissions.AddRangeAsync(rolePermissions);

        await context.SaveChangesAsync();
    }
}