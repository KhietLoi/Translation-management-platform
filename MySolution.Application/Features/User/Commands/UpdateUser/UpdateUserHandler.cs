using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.User.Commands.UpdateUser;

/// <summary>
/// Handler for updating a user.
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
{
    private readonly ILogger<UpdateUserHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserHandler(
        ILogger<UpdateUserHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var payload =  request.Payload;
        var functionName =  $"{nameof(UpdateUserHandler)}";
        _logger.LogInformation(functionName);        
        var response = new UpdateUserResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            var user = await _unitOfWork.User
                .GetUserWithRolesAsync(request.Id);

            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            if (await _unitOfWork.User.ExistsByEmailOrUsernameAsync(
                    payload.Email,
                    payload.Username,
                    request.Id))
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
            user.IsActive = payload.IsActive;

            /*if (!string.IsNullOrWhiteSpace(payload.Password))
            {
                user.PasswordHash =
                    _passwordHasher.HashPassword(payload.Password);
            }*/

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

            response.Data = new UpdateUserData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
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