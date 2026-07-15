using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Features.User.Events;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.User.Commands.CreateUser;

/// <summary>
/// Handler for creating a new user.
/// </summary>
public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateUserHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMediator  _mediator;

    public CreateUserHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<CreateUserHandler> logger,
        IPasswordHasher passwordHasher,
        IMediator mediator
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _mediator = mediator;
    }
    
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateUserHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateUserResponse();
        
        try
        {
            if (await _unitOfWork.User.ExistsByEmailOrUsernameAsync(payload.Email, payload.Username))
            {
                response.ErrorMessage = "Username or Email already exists."; 
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
         
            // Check Roles
            var roles = await _unitOfWork.Role
                .Where(x => payload.RoleIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
            if (roles.Count != payload.RoleIds.Count)
            {
                response.ErrorMessage = "One or more roles do not exist.";
                response.WithStatus(HttpStatusCode.BadRequest);

                return response;
            }
            
            //Create User
            var user = new Domain.Entities.User
            {
                Id = Guid.CreateVersion7(),
                Username = payload.Username,
                Email = payload.Email,
                PasswordHash = _passwordHasher.HashPassword(payload.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
            };
            //Add User
            await _unitOfWork.User.Add(user);
            //Asign Roles
            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            }
            await _unitOfWork.SaveAsync(cancellationToken);
            //Email
            _logger.LogInformation("Publishing UserCreatedEvent for {Email}", user.Email);
            await _mediator.Publish(new UserCreatedEvent(user.Id, user.Username, user.Email), cancellationToken);
            //Return Data
            response.Data = new CreateUserData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
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