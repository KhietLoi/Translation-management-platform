using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Common.Models;

namespace MySolution.Infrastructure.Authentication;

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;

    public ApiKeyValidator
    (
        IUnitOfWork unitOfWork,
        IHashService hashService
    )
    {
        _unitOfWork = unitOfWork;
        _hashService = hashService;
    }

    public async Task<ApiKeyContext?> ValidateAsync(string rawApiKey, CancellationToken cancellationToken)
    {
        var hash = _hashService.ComputeHash(rawApiKey);
        var apiKey = await _unitOfWork.ApiKey.GetByHashAsync(hash, cancellationToken);
        if (apiKey == null)
        {
            return null;
        }
        
        if (apiKey.RevokedAt.HasValue)
        {
            return null;
        }

        if (
            apiKey.ExpiresAt.HasValue &&
            apiKey.ExpiresAt.Value < DateTime.UtcNow
        )
        {
            return null;
        }

        return new ApiKeyContext
        {
            ApiKeyId = apiKey.Id,
            ApplicationId = apiKey.ApplicationId,
            Permissions = apiKey.Permissions
                .Select(x => x.Permission)
                .ToList()
        };
    }
}