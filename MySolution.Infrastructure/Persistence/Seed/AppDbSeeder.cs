namespace MySolution.Infrastructure.Persistence.Seed;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await RoleSeeder.SeedAsync(context);

        await PermissionSeeder.SeedAsync(context);

        await RolePermissionSeeder.SeedAsync(context);

        await UserSeeder.SeedAsync(context);

        await UserRoleSeeder.SeedAsync(context);
    }
}