using MySolution.Application.Common.Models;

namespace MySolution.Application.Common.Interfaces.Authentication;

public interface IApiKeyValidator
{ 
    Task<ApiKeyContext?> ValidateAsync(string rawApiKey, CancellationToken cancellationToken = default);
}