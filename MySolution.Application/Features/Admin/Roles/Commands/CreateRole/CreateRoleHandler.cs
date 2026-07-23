using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Roles.Commands.CreateRole;

/// <summary>
///     Handler for creating a new role.
/// </summary>
public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, CreateRoleResponse>
{
    private readonly ILogger<CreateRoleHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<CreateRoleHandler> logger
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CreateRoleResponse> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateRoleHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateRoleResponse();

        try
        {
            // Validate if the role name already exists
            if (await _unitOfWork.Role.ExistsByNameAsync(payload.Name))
            {
                response.ErrorMessage = "Role name already exists";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            // Create Role
            var role = new Role
            {
                Id = Guid.CreateVersion7(),
                Name = request.Payload.Name,
                Description = request.Payload.Description
            };

            await _unitOfWork.Role.Add(role);
            await _unitOfWork.SaveAsync(cancellationToken);
            response.Data = new CreateRoleData
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleDescription = role.Description,
                CreatedAt = DateTime.UtcNow
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