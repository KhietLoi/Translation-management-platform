using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IHashService _hashService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<LoginHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecurityStampService _tokenSecurityService;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler
    (
        ILogger<LoginHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IHashService hashService,
        ISecurityStampService tokenSecurityService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _hashService = hashService;
        _tokenSecurityService = tokenSecurityService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(LoginHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new LoginResponse();

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
            var verify = _passwordHasher.VerifyPassword(payload.Password, user.PasswordHash);
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
                response.Data = new LoginResult
                {
                    Email = user.Email,
                    IsEmailVerified = false
                };
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            //Create Jti
            var jti = Guid.CreateVersion7().ToString();
            // Generate JWT
            var accessToken = _jwtService.GenerateJwtToken(user, jti);
            // Generate RefreshToken
            var refreshToken = _jwtService.GenerateRefreshToken();
            var tokenHash = _hashService.ComputeSha256(refreshToken);
            // Save RefreshToken to database
            await _unitOfWork.RefreshToken.Add(
                new Domain.Entities.RefreshToken
                {
                    Id = Guid.CreateVersion7(),
                    UserId = user.Id,
                    TokenHash = tokenHash,
                    CreatedAt = DateTime.UtcNow,
                    Jti = jti,
                    ExpiredAt = _jwtService.GetRefreshTokenExpirationDate()
                });
            await _unitOfWork.SaveAsync(cancellationToken);
            await _tokenSecurityService.SetSecurityStampAsync(user.Id, user.SecurityStamp);

            response.Data = new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAtAccessToken = _jwtService.GetAccessTokenExpirationDate(),
                IsEmailVerified = user.IsEmailVerified,
                Email = user.Email
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