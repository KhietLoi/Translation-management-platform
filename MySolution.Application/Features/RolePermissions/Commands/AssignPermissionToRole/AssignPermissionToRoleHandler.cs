using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.RolePermissions.Commands.AssignPermissionToRole;

/// <summary>
/// Handler for the AssignPermissionToRoleCommand, responsible for assigning a permission to a role.
/// </summary>
public class AssignPermissionToRoleHandler : IRequestHandler<AssignPermissionToRoleCommand, AssignPermissionToRoleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignPermissionToRoleHandler> _logger;

    public AssignPermissionToRoleHandler(IUnitOfWork unitOfWork, ILogger<AssignPermissionToRoleHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<AssignPermissionToRoleResponse> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(AssignPermissionToRoleHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new AssignPermissionToRoleResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            //Check Role
            var role = await _unitOfWork.Role.GetByIdAsync(payload.RoleId);

            if (role == null)
            {
                response.ErrorMessage = "Role not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check Permission
            var permission = await _unitOfWork.Permission.GetPermissionByIdAsync(payload.PermissionId);
            
            if (permission == null)
            {
                response.ErrorMessage = "Permission not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check if the role is already assigned to the permission
            bool existed = await _unitOfWork.RolePermission.ExistsAsync(role.Id, permission.Id);
            
            if (existed)
            {
                response.ErrorMessage = "Role is already assigned to this permission";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            //Create RolePermission
            var rolePermission = new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permission.Id
            };
            //Save
            await _unitOfWork.RolePermission.Add(rolePermission);
            await _unitOfWork.SaveAsync(cancellationToken);
            //Response
            response.Data = new AssignPermissionToRoleData
            {
                RoleId = role.Id,
                RoleName = role.Name,
                PermissionId = permission.Id,
                PermissionCode = permission.Code
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