using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Application.Constants;
using MySolution.Domain.Entities;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.Register;

/// <summary>
/// Handler for the RegisterCommand, responsible for processing user registration requests.
/// </summary>
public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly ILogger<RegisterHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMessageSender _messageSender;
    private readonly IEmailVerificationTokenService _emailVerificationTokenService;
    
    public RegisterHandler
    (
        ILogger<RegisterHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IMessageSender messageSender,
        IEmailVerificationTokenService emailVerificationTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _messageSender = messageSender;
        _emailVerificationTokenService = emailVerificationTokenService;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(RegisterHandler)}";
        _logger.LogInformation(functionName);
        var response = new RegisterResponse();

        try
        {
            if (await _unitOfWork.User.ExistsByEmailOrUsernameAsync(payload.Email, payload.Username))
            {
                response.ErrorMessage = "Username and Email already exist.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // user role default
            var role = await _unitOfWork.Role.GetByNameAsync(RoleConstants.User);
            if (role is null)
            {
                response.ErrorMessage = "Default role not found.";
                return response;
            }

            var user = new Domain.Entities.User
            {
                Id = Guid.CreateVersion7(),
                Username = payload.Username,
                Email = payload.Email,
                PasswordHash = _passwordHasher.HashPassword(payload.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.User.Add(user);
            user.UserRoles.Add(new UserRole
            {
                //UserId = user.Id,
                RoleId = role.Id
            });
            await _unitOfWork.SaveAsync(cancellationToken);
            var token = _emailVerificationTokenService.GenerateVerificationToken(user.Id, user.Email);
            //Email
            _logger.LogInformation("Publishing UserCreatedEvent for {Email}", user.Email);
            await _messageSender.SendMessage<SendVerifyEmailEvent>(
                new SendVerifyEmailEvent
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
                },
                cancellationToken);
            response.Data = new RegisterResult
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email
            };
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.Created);
            return response;
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