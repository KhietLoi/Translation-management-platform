using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.ApiKey.Command.RevokeApiKey;

public class RevokeApiKeyHandler : IRequestHandler<RevokeApiKeyCommand, RevokeApiKeyResponse>
{
    private readonly ILogger<RevokeApiKeyHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public RevokeApiKeyHandler
    (
        ILogger<RevokeApiKeyHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in RevokeApiKeyCommand, RevokeApiKeyResponse>

    public async Task<RevokeApiKeyResponse> Handle(RevokeApiKeyCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(RevokeApiKeyHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new RevokeApiKeyResponse();

        try
        {
            var apikey = await _unitOfWork.ApiKey
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == request.ApiKeyId, cancellationToken);
            if (apikey == null)
            {
                response.ErrorMessage = "API key not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

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