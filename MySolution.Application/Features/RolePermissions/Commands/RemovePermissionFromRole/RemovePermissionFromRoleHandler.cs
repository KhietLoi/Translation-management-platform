using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

namespace MySolution.Application.Features.RolePermissions.Commands.RemovePermissionFromRole;

/// <summary>
/// Handler for removing a permission from a role.
/// </summary>
public class RemovePermissionFromRoleHandler : IRequestHandler<RemovePermissionFromRoleCommand, RemovePermissionFromRoleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemovePermissionFromRoleHandler> _logger;

    public RemovePermissionFromRoleHandler(IUnitOfWork unitOfWork, ILogger<RemovePermissionFromRoleHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<RemovePermissionFromRoleResponse> Handle(RemovePermissionFromRoleCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(RemovePermissionFromRoleHandler)}";
        _logger.LogInformation(functionName);
        var response = new RemovePermissionFromRoleResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };
    
        try
        {
            // Check if the role has the permission
            var rolePermission = await _unitOfWork.RolePermission.GetAsync(request.RoleId, request.PermissionId);

            if (rolePermission == null)
            {
                response.ErrorMessage = "Role does not have this permission";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // Remove the permission from the role
            _unitOfWork.RolePermission.Delete(rolePermission);
            // Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            // Response
            response.Data = new RemovePermissionFromRoleData
            {
                RoleId = rolePermission.RoleId,
                PermissionId = rolePermission.PermissionId
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = ex.Message; 
        }
        return response;
    }
}