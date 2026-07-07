using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Persistence.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync(x => x.Username == "admin"))
        {
            return;
        }

        var passwordHasher = new PasswordHasher<User>();

        var admin = new User
        {
            Id = SeedConstants.AdminUserId,
            Username = "admin",
            Email = "admin@gmail.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Hash password after creating the user
        admin.PasswordHash = passwordHasher.HashPassword(
            admin,
            "Admin@123");

        await context.Users.AddAsync(admin);

        await context.SaveChangesAsync();
    }
}