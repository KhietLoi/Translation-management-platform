using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

    public async Task<AssignApiKeyPermissionResponse> Handle(
    AssignApiKeyPermissionCommand request,
    CancellationToken cancellationToken)
{
    var functionName = $"{nameof(AssignApiKeyPermissionHandler)} =>";
    var response = new AssignApiKeyPermissionResponse();

    try
    {
        var apikey = await _unitOfWork.ApiKey
            .GetAll()
            .Where(x => x.Id == request.ApiKeyId)
            .ToListAsync(cancellationToken);

        if (!apikey.Any())
        {
           _logger.LogInformation("No permissions found for {FunctionName}", functionName);
           
            response.ErrorMessage = "API key not found.";
            response.WithStatus(HttpStatusCode.NotFound);
            return response;
        }
        
        var oldPermissions = await _unitOfWork.ApiKeyPermission
            .GetAll()
            .Where(x => x.ApiKeyId == request.ApiKeyId)
            .ToListAsync(cancellationToken);
        if (oldPermissions.Any())
        {
            _logger.LogInformation("API permissions found for {FunctionName}", functionName);
            
            _unitOfWork.ApiKeyPermission.DeleteRange(oldPermissions);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
        
        var permissions = request.Payload.Permissions
            .Distinct()
            .Select(x => new ApiKeyPermission
            {
                Id = Guid.NewGuid(),
                ApiKeyId = request.ApiKeyId,
                Permission = x
            })
            .ToList();
        
        if (permissions.Any())
        {
            await _unitOfWork.ApiKeyPermission.AddRange(permissions);
            await _unitOfWork.SaveAsync(cancellationToken);
        }
        
        response.Data = new AssignApiKeyPermissionData
        {
            ApiKeyId = apikey.First().Id,
            Permissions = permissions
                .Select(x => x.Permission)
                .ToList()
        };

        _logger.LogInformation("Assign API permissions for {FunctionName}", functionName);
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