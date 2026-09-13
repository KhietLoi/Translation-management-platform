using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.Roles.Commands.UpdateRole;

/// <summary>
///     Handler for updating a role.
/// </summary>
public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    private readonly ILogger<UpdateRoleHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleHandler
    (
        ILogger<UpdateRoleHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateRoleResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateRoleHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateRoleResponse();

        try
        {
            // Check if role exists
            var role = await _unitOfWork.Role
                .GetAll()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            
            if (role == null)
            {
                _logger.LogInformation("{FunctionName} Role with ID {RoleId} not found.", functionName, request.Id);
                
                response.ErrorMessage = "Role not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            // Check role name already exists
            var existingRole = await _unitOfWork.Role
                .GetAll()
                .FirstOrDefaultAsync(r => r.Name == payload.Name, cancellationToken);
       
            if (existingRole != null && existingRole.Id != role.Id)
            {
                _logger.LogInformation("{FunctionName} Role name {RoleName} already exists.", functionName, payload.Name);
                
                response.ErrorMessage = "Role name already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            // Update
            role.Name = payload.Name;
            role.Description = payload.Description;
            
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new UpdateRoleData
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleDescription = role.Description,
                UpdatedAt = DateTime.UtcNow
            };
            
            _logger.LogInformation("{FunctionName} Role updated successfully.", functionName);
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