using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
using MySolution.Domain.Enums;
using Shared.MassTransit.Core;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Admin.User.Commands.CreateUser;

/// <summary>
///     Handler for creating a new user.
/// </summary>
public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly ILogger<CreateUserHandler> _logger;
    private readonly ISendEndpointCustomProvider _messageSender;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<CreateUserHandler> logger,
        IPasswordHasher passwordHasher,
        IPasswordResetTokenService passwordResetTokenService,
        ISendEndpointCustomProvider messageSender
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _passwordResetTokenService = passwordResetTokenService;
        _messageSender = messageSender;
    }

    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(CreateUserHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new CreateUserResponse();

        try
        {
            if (await _unitOfWork.User.ExistsByEmailOrUsernameAsync(payload.Email, payload.Username, cancellationToken: cancellationToken))
            {
                response.ErrorMessage = "Username or Email already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            // Check Roles
            var roles = await _unitOfWork.Role
                .Where(x => payload.RoleIds != null && payload.RoleIds.Contains(x.Id))
                .ToListAsync(cancellationToken);
            if (payload.RoleIds != null && roles.Count != payload.RoleIds.Count)
            {
                response.ErrorMessage = "One or more roles do not exist.";
                response.WithStatus(HttpStatusCode.BadRequest);

                return response;
            }

            //Create User
            var temporaryPassword = Guid.CreateVersion7().ToString();
            var passwordHash = _passwordHasher.HashPassword(temporaryPassword);
            
            var user = new Domain.Entities.User
            {
                Id = Guid.CreateVersion7(),
                Username = payload.Username,
                Email = payload.Email,
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                Status = UserStatus.NonActive,
                CreatedAt = DateTime.UtcNow
            };  
            
            //Add User
            await _unitOfWork.User.Add(user);
            
            //Asign Roles
            foreach (var role in roles)
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });
            
            //Create empty user profile.
            user.Profile = new UserProfile
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.SaveAsync(cancellationToken);

            var token = _passwordResetTokenService.GenerateResetToken(user.Id, user.Email, user.Username, user.PasswordVersion);

            await _messageSender.SendMessage<SendSetUpPasswordEmailEvent>(
                new SendSetUpPasswordEmailEvent
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
                }, cancellationToken);
            
            response.Data = new CreateUserData
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsEmailVerified = true,
                Status = UserStatus.NonActive
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