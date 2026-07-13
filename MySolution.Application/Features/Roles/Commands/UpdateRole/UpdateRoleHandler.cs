using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Handler for updating a role.
/// </summary>
public class UpdateRoleHandler : IRequestHandler<UpdateRoleCommand, UpdateRoleResponse>
{
    private readonly ILogger<UpdateRoleHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateRoleHandler
    (
        ILogger<UpdateRoleHandler> logger,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<UpdateRoleResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateRoleHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateRoleResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            // Check if role exists
            var role = await _unitOfWork.Role.GetByIdAsync(request.Id);
            if (role == null)
            {
                response.ErrorMessage = "Role not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            // Check role name already exists
            var existingRole = await _unitOfWork.Role.GetByNameAsync(payload.Name);
            
            if (existingRole != null && existingRole.Id != role.Id)
            {
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
                UpdatedAt = _dateTimeProvider.UtcNow
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