using System.Security.Cryptography;
using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Infrastructure.Authentication;

public class ApiKeyGenerator : IApiKeyGenerator
{
    public string GenerateApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var secret = Convert.ToHexString(bytes);

        return $"ms_live_{secret}";
    }
}