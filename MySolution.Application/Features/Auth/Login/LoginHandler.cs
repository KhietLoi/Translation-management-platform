using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;

namespace MySolution.Application.Features.Auth.Login;

public class LoginHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly ILogger<LoginHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public LoginHandler
    (
        ILogger<LoginHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(LoginHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new LoginResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            // Find user by username
            var user = await _unitOfWork.User.GetUserWithRolesAsync(payload.Username);
            if (user == null)
            {
                _logger.LogWarning("{FunctionName} User not found: {Username}", functionName, payload.Username);
                response.ErrorMessage = "Username or Password is incorrect.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }

            // Check account status
            if (!user.IsActive)
            {
                _logger.LogWarning("{FunctionName} User inactive: {Username}", functionName, payload.Username);
                response.ErrorMessage = "Account is locked.";
                response.WithStatus(HttpStatusCode.Forbidden);
                return response;
            }

            // Check Password
            var verify = _passwordHasher.VerifyPassword(
                payload.Password,
                user.PasswordHash);
            if (!verify)
            {
                _logger.LogWarning("{FunctionName} Invalid password: {Username}", functionName, payload.Username);
                response.ErrorMessage = "Username or Password is incorrect.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            //Check IsEmailVerified
            if (!user.IsEmailVerified)
            {
                _logger.LogWarning("{FunctionName} User not verified: {Username}", functionName, payload.Username);
                response.ErrorMessage = "User is not verified.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            // Generate JWT
            var accessToken = _jwtService.GenerateJwtToken(user);

            // Generate RefreshToken
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save RefreshToken to database
            await _unitOfWork.RefreshToken.Add(
                new Domain.Entities.RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiredAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false
                });

            await _unitOfWork.SaveAsync(cancellationToken);
            
            response.Data = new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15) //after
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);

            _logger.LogInformation("{FunctionName} User login successfully: {Username}", functionName, user.Username);
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