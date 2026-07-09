using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Auth.Register;

/// <summary>
/// Handler for the RegisterCommand, responsible for processing user registration requests.
/// </summary>
public class RegisterHandler
    : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly ILogger<RegisterHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RegisterHandler(
        ILogger<RegisterHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var response = new RegisterResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            if (await _unitOfWork.User.ExistsByUsernameAsync(payload.Username))
            {
                response.ErrorMessage = "Username already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            if (await _unitOfWork.User.ExistsByEmailAsync(payload.Email))
            {
                response.ErrorMessage = "Email already exists.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var role = await _unitOfWork.Role.GetByNameAsync("User");

            if (role is null)
            {
                response.ErrorMessage = "Default role not found.";
                return response;
            }

            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                Username = payload.Username,
                Email = payload.Email,
                PasswordHash = _passwordHasher.HashPassword(payload.Password),
                IsActive = true,
                CreatedAt = _dateTimeProvider.UtcNow
            };

            await _unitOfWork.User.Add(user);

            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });

            await _unitOfWork.SaveAsync(cancellationToken);
            
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
            _logger.LogError(ex, "Register failed");
            response.ErrorMessage = ex.Message;
            return response;
        }
    }
}