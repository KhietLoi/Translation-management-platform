using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.Permission.Commands.DeletePermission;

/// <summary>
///     Handler for deleting a permission.
/// </summary>
public class DeletePermissionHandler : IRequestHandler<DeletePermissionCommand, DeletePermissionResponse>
{
    private readonly ILogger<DeletePermissionHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePermissionHandler(IUnitOfWork unitOfWork, ILogger<DeletePermissionHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeletePermissionResponse> Handle(DeletePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeletePermissionHandler)}";
        _logger.LogInformation(functionName);
        var response = new DeletePermissionResponse();

        try
        {
            // Get permission by Id
            var permission = await _unitOfWork.Permission
                .GetAll()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            
            // Check if permission exists
            if (permission == null)
            {
                _logger.LogInformation("{FunctionName} Permission not found for Id: {PermissionId}", functionName, request.Id);
                
                response.ErrorMessage = "Permission not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

           
            _unitOfWork.Permission.Delete(permission);
            await _unitOfWork.SaveAsync(cancellationToken);
                
            // Response
            response.Data = new DeletePermissionData
            {
                Id = permission.Id,
                Code = permission.Code,
                Description = permission.Description
            };
            
            _logger.LogInformation("{FunctionName} Permission deleted successfully for Id: {PermissionId}", functionName, request.Id);
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
}