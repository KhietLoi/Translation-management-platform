using System.Security.Cryptography;
using System.Text;
using MySolution.Application.Common.Interfaces;

namespace MySolution.Infrastructure.Authentication;

public class HashService : IHashService
{
    public string ComputeSha256(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}