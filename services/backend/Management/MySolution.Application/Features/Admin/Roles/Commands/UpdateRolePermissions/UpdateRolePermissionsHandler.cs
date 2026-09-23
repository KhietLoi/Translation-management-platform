using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.Admin.User.Queries.GetUserById;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRolePermissions;

/// <summary>
///     Handler for updating role permissions.
/// </summary>
public class UpdateRolePermissionsHandler : IRequestHandler<UpdateRolePermissionsCommand, UpdateRolePermissionsResponse>
{
    private readonly ILogger<UpdateRolePermissionsHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionCacheService _permissionCacheService;

    public UpdateRolePermissionsHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<UpdateRolePermissionsHandler> logger,
        IPermissionCacheService permissionCacheService
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _permissionCacheService = permissionCacheService;
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
            var role = await _unitOfWork.Role
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == payload.RoleId, cancellationToken);
            if (role is null)
            {
                _logger.LogInformation("{FunctionName} Role with ID {RoleId} not found.", functionName, payload.RoleId);
                response.ErrorMessage = "Role not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var currentRolePermissions = await _unitOfWork.RolePermission
                .GetAll()
                .Where(x => x.RoleId == payload.RoleId)
                .ToListAsync(cancellationToken);
            
            var currentPermissionIds = currentRolePermissions.Select(x => x.PermissionId).ToHashSet();
            var newPermissionIds = payload.PermissionIds.Distinct().ToHashSet();
            var permissionIdsToAdd = newPermissionIds.Except(currentPermissionIds).ToList(); 

            // Permissions need remove
            var rolePermissionsToRemove = currentRolePermissions
                .Where(x => !newPermissionIds.Contains(x.PermissionId))
                .ToList();
            if (permissionIdsToAdd.Count > 0)
            {
                var permissions = await _unitOfWork.Permission
                    .GetAll()
                    .Where(x => permissionIdsToAdd.Contains(x.Id))
                    .ToListAsync(cancellationToken);
                
                var foundPermissionIds = permissions.Select(x => x.Id).ToHashSet();
                var invalidPermissions = permissionIdsToAdd.Except(foundPermissionIds).ToList();
                if (invalidPermissions.Count > 0)
                {
                    response.ErrorMessage = "One or more permissions do not exist.";
                    response.WithStatus(HttpStatusCode.BadRequest);
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
                
                await _unitOfWork.RolePermission.AddRange(entities);
            }

            // Remove permissions
            if (rolePermissionsToRemove.Count > 0)
            {
                _unitOfWork.RolePermission.DeleteRange(rolePermissionsToRemove);
            }
            
            await _unitOfWork.SaveAsync(cancellationToken);
            
            //Update Redis:
            var userIds =
                await _unitOfWork.UserRole
                    .GetAll()
                    .Where(x => x.RoleId == payload.RoleId)
                    .Select(x => x.UserId)
                    .ToListAsync(cancellationToken);

            foreach (var userId in userIds)
            {
                await _permissionCacheService.RemoveAsync(userId);
            }
           
            var updatedPermissions = await _unitOfWork.RolePermission
                .GetAll()
                .AsNoTracking()
                .Where(x => x.RoleId == payload.RoleId)
                .Include(x => x.Permission)
                .ToListAsync(cancellationToken);
            
            response.Data = new UpdateRolePermissionsData
                {
                    RoleId = role.Id,
                    RoleName = role.Name,

                    Permissions =
                        updatedPermissions
                            .Select(x =>
                                new PermissionData
                                {
                                    PermissionId = x.Permission.Id,
                                    PermissionCode = x.Permission.Code,
                                    PermissionDescription = x.Permission.Description
                                })
                            .ToList()
                };
            
            _logger.LogInformation("{FunctionName} Successfully updated role permissions for Role ID {RoleId}.", functionName, payload.RoleId);
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