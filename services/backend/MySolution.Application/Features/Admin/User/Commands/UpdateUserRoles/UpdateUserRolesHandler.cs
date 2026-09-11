using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Admin.User.Commands.UpdateUserRoles;

public class UpdateUserRolesHandler : IRequestHandler<UpdateUserRolesCommand, UpdateUserRolesResponse>
{
    private readonly ILogger<UpdateUserRolesHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRolesHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateUserRolesHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UpdateUserRolesResponse> Handle(UpdateUserRolesCommand request,
        CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateUserRolesHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateUserRolesResponse();

        try
        {
            // Check user exists
            var user = await _unitOfWork.User.GetByIdAsync(payload.UserId);
            if (user is null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            // Current roles
            var currentUserRoles = await _unitOfWork.UserRole.GetByUserIdAsync(payload.UserId);
            var currentRoleIds = currentUserRoles.Select(x => x.RoleId).ToHashSet();
            var newRoleIds = payload.RoleIds.Distinct().ToHashSet();
            // Roles need add
            var roleIdsToAdd = newRoleIds.Except(currentRoleIds).ToList();
            // Roles need remove
            var userRolesToRemove =
                currentUserRoles
                    .Where(x =>
                        !newRoleIds.Contains(
                            x.RoleId))
                    .ToList();
            // Validate roles
            if (roleIdsToAdd.Count > 0)
            {
                var roles = await _unitOfWork.Role.GetByIdsAsync(roleIdsToAdd);
                var foundRoleIds = roles.Select(x => x.Id).ToHashSet();
                var invalidRoles = roleIdsToAdd.Except(foundRoleIds).ToList();
                if (invalidRoles.Count > 0)
                {
                    response.ErrorMessage = "One or more roles do not exist.";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                var entities =
                    roles.Select(x =>
                            new UserRole
                            {
                                UserId = payload.UserId,
                                RoleId = x.Id
                            })
                        .ToList();
                await _unitOfWork.UserRole.AddRange(entities);
            }

            // Remove roles
            if (userRolesToRemove.Count > 0)
                _unitOfWork.UserRole
                    .DeleteRange(userRolesToRemove);

            await _unitOfWork.SaveAsync(cancellationToken);
            // Get updated roles
            var updatedRoles = await _unitOfWork.UserRole.GetByUserIdWithRoleAsync(payload.UserId);
            response.Data =
                new UpdateUserRolesData
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Roles =
                        updatedRoles
                            .Select(x =>
                                new RoleData
                                {
                                    RoleId = x.Role!.Id,

                                    RoleName = x.Role.Name
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