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
    private readonly IDateTimeProvider _dateTimeProvider;

    public LoginHandler
    (
        ILogger<LoginHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        IDateTimeProvider dateTimeProvider
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
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
            // Tìm User
            var user = await _unitOfWork.User
                .GetUserWithRolesAsync(payload.Username);

            if (user == null)
            {
                _logger.LogWarning(
                    "{FunctionName} User not found: {Username}",
                    functionName,
                    payload.Username);

                response.ErrorMessage = "Username hoặc Password không đúng.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            // Kiểm tra trạng thái tài khoản
            if (!user.IsActive)
            {
                _logger.LogWarning(
                    "{FunctionName} User inactive: {Username}",
                    functionName,
                    payload.Username);

                response.ErrorMessage = "Tài khoản đã bị khóa.";

                response.WithStatus(HttpStatusCode.Forbidden);

                return response;
            }

            // Kiểm tra Password
            var verify = _passwordHasher.VerifyPassword(
                payload.Password,
                user.PasswordHash);

            if (!verify)
            {
                _logger.LogWarning(
                    "{FunctionName} Invalid password: {Username}",
                    functionName,
                    payload.Username);

                response.ErrorMessage = "Username hoặc Password không đúng.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            // Sinh JWT
            var accessToken =
                _jwtService.GenerateJwtToken(user);

            // Sinh RefreshToken
            var refreshToken =
                _jwtService.GenerateRefreshToken();

            // Lưu RefreshToken
            await _unitOfWork.RefreshToken.Add(
                new Domain.Entities.RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken,
                    CreatedAt = _dateTimeProvider.UtcNow,
                    ExpiredAt = _dateTimeProvider.UtcNow.AddDays(7),
                    IsRevoked = false
                });

            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new LoginResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = _dateTimeProvider.UtcNow.AddMinutes(15)
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);

            _logger.LogInformation(
                "{FunctionName} User login successfully: {Username}",
                functionName,
                user.Username);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FunctionName} Unexpected error",
                functionName);

            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}