namespace MySolution.Infrastructure.Persistence.Seed;

public static class SeedConstants
{
    // Roles
    public static readonly Guid AdminRoleId =
        Guid.Parse("11111111-1111-1111-1111-111111111111");

    public static readonly Guid UserRoleId =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    // Admin User
    public static readonly Guid AdminUserId =
        Guid.Parse("33333333-3333-3333-3333-333333333333");

    // Permissions
    public static readonly Guid UserReadPermissionId =
        Guid.Parse("44444444-4444-4444-4444-444444444441");

    public static readonly Guid UserCreatePermissionId =
        Guid.Parse("44444444-4444-4444-4444-444444444442");

    public static readonly Guid UserUpdatePermissionId =
        Guid.Parse("44444444-4444-4444-4444-444444444443");

    public static readonly Guid UserDeletePermissionId =
        Guid.Parse("44444444-4444-4444-4444-444444444444");

    public static readonly Guid RoleReadPermissionId =
        Guid.Parse("55555555-5555-5555-5555-555555555551");

    public static readonly Guid RoleCreatePermissionId =
        Guid.Parse("55555555-5555-5555-5555-555555555552");

    public static readonly Guid RoleUpdatePermissionId =
        Guid.Parse("55555555-5555-5555-5555-555555555553");

    public static readonly Guid RoleDeletePermissionId =
        Guid.Parse("55555555-5555-5555-5555-555555555554");
}