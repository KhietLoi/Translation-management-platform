using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.GetCurrentUser;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<GetCurrentUserHandler> _logger;
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;

    public GetCurrentUserHandler
    (
        ICurrentUser currentUser,
        ILogger<GetCurrentUserHandler> logger,
        IPermissionService permissionService,
        IUnitOfWork unitOfWork
    )
    {
        _currentUser = currentUser;
        _logger = logger;
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }
        
    public async Task<GetCurrentUserResponse> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetCurrentUserHandler)}";
        _logger.LogInformation(functionName);
        var response = new GetCurrentUserResponse();

        try
        {
            //var user = await _unitOfWork.User.GetByIdAsync(_currentUser.UserId);
            var user = await _unitOfWork.User
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);
            
            if (user == null)
            {
                _logger.LogInformation($"{functionName} User not found. UserId: {_currentUser.UserId}");
                
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var permission = await _permissionService.GetPermissionsAsync(_currentUser.UserId);

            response.Data = new GetCurrentUserData
            {
                UserId = user.Id,
                UserName = user.Username,
                Email = user.Email,
                Roles = _currentUser.Roles.ToList(),
                Permissions = permission.ToList()
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