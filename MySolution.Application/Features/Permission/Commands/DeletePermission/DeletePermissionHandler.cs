using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Permission.Commands.DeletePermission;

/// <summary>
/// Handler for deleting a permission.
/// </summary>
public class DeletePermissionHandler : IRequestHandler<DeletePermissionCommand, DeletePermissionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePermissionHandler> _logger;

    public DeletePermissionHandler(IUnitOfWork unitOfWork, ILogger<DeletePermissionHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<DeletePermissionResponse> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeletePermissionHandler)}";
        _logger.LogInformation(functionName);
        var response = new DeletePermissionResponse();
        
        try
        {
            // Get permission by Id
            var permission = await _unitOfWork.Permission.GetPermissionByIdAsync(request.Id);
            // Check if permission exists
            if (permission == null)
            {
                response.ErrorMessage = "Permission not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            // Delete
            _unitOfWork.Permission.Delete(permission);
            // Save
            await _unitOfWork.SaveAsync(cancellationToken);
            // Response
            response.Data = new DeletePermissionData
            {
                Id = permission.Id,
                Code = permission.Code,
                Description = permission.Description
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
}