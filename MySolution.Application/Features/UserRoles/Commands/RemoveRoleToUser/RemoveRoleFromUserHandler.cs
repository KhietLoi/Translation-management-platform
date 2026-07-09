using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.Permission.Commands.DeletePermission;

namespace MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

public class RemoveRoleFromUserHandler : IRequestHandler<RemoveRoleFromUserCommand, RemoveRoleFromUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveRoleFromUserHandler> _logger;

    public RemoveRoleFromUserHandler(IUnitOfWork unitOfWork, ILogger<RemoveRoleFromUserHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    
    public async Task<RemoveRoleFromUserResponse> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(RemoveRoleFromUserHandler)}";
        _logger.LogInformation(functionName);
        var response = new RemoveRoleFromUserResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            // Check if user exists
            var userRole = await _unitOfWork.UserRole.GetAsync(request.UserId, request.RoleId);
            
            if (userRole == null)
            {
                response.ErrorMessage = "User does not have this role";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // Remove role from user
            _unitOfWork.UserRole.Delete(userRole);
            // Save
            await _unitOfWork.SaveAsync(cancellationToken);
            // Return value
            response.Data = new RemoveRoleFromUserData
            {
                UserId = request.UserId,
                RoleId = request.RoleId
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