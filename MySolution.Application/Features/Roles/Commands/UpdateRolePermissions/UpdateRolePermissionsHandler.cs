using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.User.Queries.GetUserById;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Roles.Commands.UpdateRolePermissions;

/// <summary>
/// Handler for updating role permissions.
/// </summary>
public class UpdateRolePermissionsHandler : IRequestHandler<UpdateRolePermissionsCommand, UpdateRolePermissionsResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRolePermissionsHandler> _logger;

    public UpdateRolePermissionsHandler(IUnitOfWork unitOfWork, ILogger<UpdateRolePermissionsHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateRolePermissionsResponse> Handle(UpdateRolePermissionsCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateRolePermissionsHandler)}";
        _logger.LogInformation(functionName);
        var response = new UpdateRolePermissionsResponse();

        try
        {
            // Check role exists
            var role = await _unitOfWork.Role.GetByIdAsync(payload.RoleId);
            if (role is null)
            {
                response.ErrorMessage = "Role not found.";
                response.WithStatus(HttpStatusCode.NotFound);

                return response;
            }
            
            var currentRolePermissions = await _unitOfWork.RolePermission.GetByRoleIdAsync(payload.RoleId);
            var currentPermissionIds = currentRolePermissions.Select(x => x.PermissionId).ToHashSet();
            var newPermissionIds = payload.PermissionIds.Distinct().ToHashSet();
            // Permissions need add
            var permissionIdsToAdd =
                newPermissionIds
                    .Except(currentPermissionIds)
                    .ToList();

            // Permissions need remove
            var rolePermissionsToRemove =
                currentRolePermissions
                    .Where(x =>
                        !newPermissionIds.Contains(
                            x.PermissionId))
                    .ToList();

            // Validate permissions exist
            if (permissionIdsToAdd.Count > 0)
            {
                var permissions = await _unitOfWork.Permission.GetByIdsAsync(permissionIdsToAdd);
                var foundPermissionIds = permissions.Select(x => x.Id).ToHashSet();
                var invalidPermissions = permissionIdsToAdd.Except(foundPermissionIds).ToList();
                if (invalidPermissions.Count > 0)
                {
                    response.ErrorMessage =
                        "One or more permissions do not exist.";

                    response.WithStatus(
                        HttpStatusCode.BadRequest);

                    return response;
                }

                var entities =
                    permissions
                        .Select(x =>
                            new RolePermission
                            {
                                RoleId = payload.RoleId,
                                PermissionId = x.Id
                            })
                        .ToList();
                await _unitOfWork.RolePermission
                    .AddRange(entities);
            }
            
            // Remove permissions
            if (rolePermissionsToRemove.Count > 0)
            {
                _unitOfWork.RolePermission
                    .DeleteRange(rolePermissionsToRemove);
            }

            await _unitOfWork.SaveAsync(cancellationToken);
            var updatedPermissions =
                await _unitOfWork.RolePermission
                    .GetByRoleIdWithPermissionAsync(
                        payload.RoleId);
            response.Data =
                new UpdateRolePermissionsData
                {
                    RoleId = role.Id,
                    RoleName = role.Name,

                    Permissions =
                        updatedPermissions
                            .Select(x =>
                                new PermissionData
                                { 
                                    PermissionId = x.Permission!.Id,
                                    PermissionCode = x.Permission.Code,
                                    PermissionDescription = x.Permission.Description
                                })
                            .ToList()
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