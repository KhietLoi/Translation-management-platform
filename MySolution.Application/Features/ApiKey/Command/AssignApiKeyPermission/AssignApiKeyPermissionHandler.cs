using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
namespace MySolution.Application.Features.ApiKey.Command.AssignApiKeyPermission;

public class AssignApiKeyPermissionHandler : IRequestHandler<AssignApiKeyPermissionCommand, AssignApiKeyPermissionResponse>
{
    private readonly ILogger<AssignApiKeyPermissionHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public AssignApiKeyPermissionHandler
    (
        ILogger<AssignApiKeyPermissionHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in AssignApiKeyPermissionCommand, AssignApiKeyPermissionResponse>

    public async Task<AssignApiKeyPermissionResponse> Handle(AssignApiKeyPermissionCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(AssignApiKeyPermissionHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new AssignApiKeyPermissionResponse();

        try
        {
            var apikey = await _unitOfWork.ApiKey.GetByIdAsync(request.ApiKeyId);
            if (apikey == null)
            {
                response.ErrorMessage = "API key not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var oldPermissions = apikey.Permissions.ToList();
            if (oldPermissions.Any())
            {
                _unitOfWork.ApiKeyPermission.DeleteRange(oldPermissions);
            }
            var permissions = request.Payload.Permissions
                .Distinct()
                .Select(x => new ApiKeyPermission
                {
                    Id = Guid.NewGuid(),
                    ApiKeyId = request.ApiKeyId,
                    Permission = x
                }).ToList();
            
            await _unitOfWork.ApiKeyPermission.AddRange(permissions);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new AssignApiKeyPermissionData
            {
                ApiKeyId = apikey.Id,
                Permissions = permissions.Select(x => x.Permission).ToList()
            };
            
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