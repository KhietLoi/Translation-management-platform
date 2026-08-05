using System.Security.Cryptography;
using System.Text;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Authentication;

public class HashService : IHashService
{
    public string ComputeHash(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    public bool Verify(string value, string hash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(hash);
        var computedHash = ComputeHash(value);
        return computedHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}