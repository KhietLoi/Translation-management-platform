using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.Permission.Commands.CreatePermission;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;

/// <summary>
/// Handler for assigning a role to a user.
/// </summary>
public class AssignRoleToUserHandler : IRequestHandler<AssignRoleToUserCommand, AssignRoleToUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignRoleToUserHandler> _logger;

    public AssignRoleToUserHandler
    (
            IUnitOfWork unitOfWork,
            ILogger<AssignRoleToUserHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<AssignRoleToUserResponse> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(AssignRoleToUserHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new AssignRoleToUserResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };
        try
        {
            //Check User
            var user = await _unitOfWork.User
                .GetByIdAsync(payload.UserId);

            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            //Check Role
            var role = await _unitOfWork.Role.GetByIdAsync(payload.RoleId);

            if (role == null)
            {
                response.ErrorMessage = "Role not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            //Check existing of UserRole
            bool existed = await _unitOfWork.UserRole.ExistsAsync(user.Id, role.Id);

            if (existed)
            {
                response.ErrorMessage = "User is already assigned to this role";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            //Add UserRole
            var useRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            //Save
            await _unitOfWork.UserRole.Add(useRole);
            await _unitOfWork.SaveAsync(cancellationToken);
            //Response
            response.Data = new AssignRoleToUserData
            {
                UserId = user.Id,
                Username = user.Username,
                RoleId = role.Id,
                RoleName = role.Name
            };
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
}