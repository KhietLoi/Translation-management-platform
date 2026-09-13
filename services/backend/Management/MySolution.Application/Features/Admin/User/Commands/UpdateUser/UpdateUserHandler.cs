using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Admin.User.Commands.UpdateUser;

/// <summary>
///     Handler for updating a user.
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly ILogger<UpdateUserHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionCacheService _permissionCacheService;

    public UpdateUserHandler
    (
        ILogger<UpdateUserHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IPermissionCacheService permissionCacheService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _permissionCacheService = permissionCacheService;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(UpdateUserHandler)}";
        _logger.LogInformation(functionName);
        var response = new UpdateUserResponse();

        try
        {
            var user = await _unitOfWork.User
                .GetAll()
                .AsSplitQuery()
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (await _unitOfWork.User.ExistsByEmailOrUsernameAsync(payload.Email, payload.Username, request.Id, cancellationToken))
            {
                response.ErrorMessage = "Username or email already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var roles = await _unitOfWork.Role
                .Where(x => payload.RoleIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
            if (roles.Count != payload.RoleIds.Count)
            {
                response.ErrorMessage = "One or more roles do not exist.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            user.Username = payload.Username;
            user.Email = payload.Email;
            user.Status = payload.Status;
            user.UserRoles.Clear();

            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
            
            await _unitOfWork.SaveAsync(cancellationToken);
            await _permissionCacheService.RemoveAsync(user.Id);
            
            response.Data = new UpdateUserData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Status = user.Status,
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