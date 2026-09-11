using Microsoft.AspNetCore.Identity;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Domain.Entities;

namespace MySolution.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(new User(), password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, passwordHash, password); 
        return result == PasswordVerificationResult.Success;
    }
}