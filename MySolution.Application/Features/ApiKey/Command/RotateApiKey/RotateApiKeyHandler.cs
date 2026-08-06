using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.ApiKey.Command.RotateApiKey;

public class RotateApiKeyHandler : IRequestHandler<RotateApiKeyCommand, RotateApiKeyResponse>
{
    private readonly ILogger<RotateApiKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IHashService _hashService;
    private readonly IApiKeyGenerator  _apiKeyGenerator;
    private readonly ICurrentUser _currentUser;

    public RotateApiKeyHandler
    (
        ILogger<RotateApiKeyHandler> logger,
		IUnitOfWork unitOfWork,
        IHashService hashService,
        IApiKeyGenerator apiKeyGenerator,
        ICurrentUser currentUserService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _hashService = hashService;
        _apiKeyGenerator = apiKeyGenerator;
        _currentUser = currentUserService;
    }

    #region Implementation of IRequestHandler<in RotateApiKeyCommand, RotateApiKeyResponse>

    public async Task<RotateApiKeyResponse> Handle(RotateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(RotateApiKeyHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new RotateApiKeyResponse();

        try
        {
            var apikey = await _unitOfWork.ApiKey.GetByIdWithPermissionsAsync(request.ApiKeyId, cancellationToken);
            if (apikey == null)
            {
                response.ErrorMessage = "API key not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            if (apikey.ExpiresAt.HasValue && apikey.ExpiresAt.Value < DateTime.UtcNow)
            {
                response.ErrorMessage = "API key has already expired.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            if (apikey.IsRevoked)
            {
                response.ErrorMessage = "API key is revoked.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var rawKey = _apiKeyGenerator.GenerateApiKey();
            var keyPrefix = rawKey.Length > 16? rawKey[..16] : rawKey;
            var hash = _hashService.ComputeHash(rawKey);
            var now = DateTime.UtcNow;
            
            var newApiKey = new Domain.Entities.ApiKey
            {
                Id = Guid.CreateVersion7(),
                ApplicationId = apikey.ApplicationId,
                Name = apikey.Name,
                KeyPrefix = keyPrefix,
                KeyHash = hash,
                CreatedAt = now,
                ExpiresAt = apikey.ExpiresAt,
                CreatedBy = _currentUser.UserId
            };
            
            await _unitOfWork.ApiKey.Add(newApiKey);
            await _unitOfWork.SaveAsync(cancellationToken);
            var permissions = apikey.Permissions
                    .Select(x => new ApiKeyPermission
                    {
                        Id = Guid.NewGuid(),
                        ApiKeyId = newApiKey.Id,
                        Permission = x.Permission
                    }).ToList();
            await _unitOfWork.ApiKeyPermission.AddRange(permissions);
            
            apikey.RevokedAt = DateTime.UtcNow;
            apikey.RevokedBy = _currentUser.UserId;
            
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}