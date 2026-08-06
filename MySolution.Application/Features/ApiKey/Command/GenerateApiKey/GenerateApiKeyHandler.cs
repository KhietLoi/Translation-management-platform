using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.ApiKey.Command.GenerateApiKey;

public class GenerateApiKeyHandler : IRequestHandler<GenerateApiKeyCommand, GenerateApiKeyResponse>
{
    private readonly ILogger<GenerateApiKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IApiKeyGenerator  _apiKeyGenerator;
    private readonly IHashService _hashService;
    private readonly ICurrentUser _currentUserService;

    public GenerateApiKeyHandler
    (
        ILogger<GenerateApiKeyHandler> logger,
		IUnitOfWork unitOfWork,
        IApiKeyGenerator apiKeyGenerator,
        IHashService hashService,
        ICurrentUser currentUserService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _apiKeyGenerator = apiKeyGenerator;
        _hashService = hashService;
        _currentUserService = currentUserService;
    }

    #region Implementation of IRequestHandler<in GenerateApiKeyCommand, GenerateApiKeyResponse>

    public async Task<GenerateApiKeyResponse> Handle(GenerateApiKeyCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GenerateApiKeyHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GenerateApiKeyResponse();

        try
        {
            var application = await _unitOfWork.Application.GetByIdAsync(request.ApplicationId);
            if (application == null)
            {
                response.ErrorMessage = "Application not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            // Check duplicate name
            var existingApiKey = await _unitOfWork.ApiKey.IsApiKeyExistsAsync(request.ApplicationId, request.Payload.Name);
            if (existingApiKey)
            {
                response.ErrorMessage = "An API key with the same name already exists for this application.";
                response.WithStatus(HttpStatusCode.Conflict);
                return response;
            }
            
            // Generate raw key:
            var rawKey = _apiKeyGenerator.GenerateApiKey();
            // Generate prefix:
            var keyPrefix = rawKey.Length >16 ? rawKey[..16] : rawKey;
            // Hash key:
            var hash = _hashService.ComputeHash(rawKey);
            // Create new ApiKey entity
            var now = DateTime.UtcNow;
            DateTime? expiresAt = request.Payload.NumofDaysExpires == 0
                ? null
                : now.AddDays(request.Payload.NumofDaysExpires);
            
            var apiKey = new Domain.Entities.ApiKey
            {
                Id = Guid.CreateVersion7(),
                ApplicationId = request.ApplicationId,
                Name = request.Payload.Name,
                KeyHash = hash,
                KeyPrefix = keyPrefix,
                ExpiresAt = expiresAt,
                CreatedAt = now,
                CreatedBy = _currentUserService.UserId
            };
            
            await _unitOfWork.ApiKey.Add(apiKey);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new GenerateApiKeyData
            {
                Id = apiKey.Id,
                Name = apiKey.Name,
                ApiKey = rawKey,
                KeyPrefix = keyPrefix,
                ExpiresAt = apiKey.ExpiresAt
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
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