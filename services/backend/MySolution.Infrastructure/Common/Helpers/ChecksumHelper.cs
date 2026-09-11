using System.Security.Cryptography;

namespace MySolution.Infrastructure.Common.Helpers;

public class ChecksumHelper
{
    public static string CalculateSha256(Stream stream)
    {
        stream.Position = 0;
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(stream);
        stream.Position = 0;

        return Convert.ToHexString(hash);
    }
}