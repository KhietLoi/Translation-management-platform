using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Roles.Commands.DeleteRole;

/// <summary>
/// Handler for deleting a role.
/// </summary>
public class DeleteRoleHandler : IRequestHandler<DeleteRoleCommand,DeleteRoleResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRoleHandler> _logger;
    public DeleteRoleHandler(IUnitOfWork unitOfWork,  ILogger<DeleteRoleHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<DeleteRoleResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(DeleteRoleHandler)}";
        _logger.LogInformation(functionName);
        var response = new DeleteRoleResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var role = await _unitOfWork.Role.GetByIdAsync(request.Id);
            
            if (role == null)
            {
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
                Description = role.Description,
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = ex.Message; 
        }
        return response;
    }
}