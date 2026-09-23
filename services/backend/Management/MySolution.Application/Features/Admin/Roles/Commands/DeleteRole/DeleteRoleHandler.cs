using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Admin.Roles.Commands.DeleteRole;

/// <summary>
///     Handler for deleting a role.
/// </summary>
public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand, DeleteRoleResponse>
{
    private readonly ILogger<DeleteRoleHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleHandler(IUnitOfWork unitOfWork, ILogger<DeleteRoleHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<DeleteRoleResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteRoleHandler)}";
        _logger.LogInformation(functionName);
        var response = new DeleteRoleResponse();

        try
        {
            var role = await _unitOfWork.Role
                .GetAll()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (role == null)
            {
                _logger.LogInformation("{FunctionName} Role with ID {RoleId} not found.", functionName, request.Id);
                response.ErrorMessage = "Role not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            _unitOfWork.Role.Delete(role);
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new DeleteRoleData
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
            
            _logger.LogInformation("{FunctionName} Role with ID {RoleId} deleted successfully.", functionName, request.Id);
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