using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.RefreshToken;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly ILogger<RefreshTokenHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public RefreshTokenHandler
    (
        ILogger<RefreshTokenHandler> logger,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IDateTimeProvider dateTimeProvider
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _dateTimeProvider = dateTimeProvider;
    }
   public async Task<RefreshTokenResponse> Handle(
    RefreshTokenCommand request,
    CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        var functionName = $"{nameof(RefreshTokenHandler)} =>";

        _logger.LogInformation(functionName);

        var response = new RefreshTokenResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {
            // Read claims from the expired access token
            var principal = _jwtService.GetPrincipalFromExpiredToken(
                payload.AccessToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
            {
                response.ErrorMessage = "Invalid access token.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            var userId = Guid.Parse(userIdClaim.Value);

            // Retrieve refresh token from the database
            var refreshToken = await _unitOfWork.RefreshToken
                .GetByTokenAsync(payload.RefreshToken);

            if (refreshToken is null)
            {
                response.ErrorMessage = "Refresh token not found.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            // Ensure the refresh token belongs to the current user
            if (refreshToken.UserId != userId)
            {
                response.ErrorMessage = "Invalid refresh token.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            // Check whether the refresh token has been revoked
            if (refreshToken.IsRevoked)
            {
                response.ErrorMessage = "Refresh token has been revoked.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            // Check whether the refresh token has expired
            if (refreshToken.ExpiredAt <= _dateTimeProvider.UtcNow)
            {
                response.ErrorMessage = "Refresh token has expired.";

                response.WithStatus(HttpStatusCode.Unauthorized);

                return response;
            }

            // Load user with roles and permissions
            var user = await _unitOfWork.User
                .GetUserWithRolesAsync(userId);

            if (user is null)
            {
                response.ErrorMessage = "User not found.";

                response.WithStatus(HttpStatusCode.NotFound);

                return response;
            }

            // Generate a new access token
            var accessToken = _jwtService.GenerateJwtToken(user);

            // Generate a new refresh token
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Revoke the current refresh token
            refreshToken.IsRevoked = true;

            // Store the newly generated refresh token
            await _unitOfWork.RefreshToken.Add(
                new Domain.Entities.RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = newRefreshToken,
                    CreatedAt = _dateTimeProvider.UtcNow,
                    ExpiredAt = _dateTimeProvider.UtcNow.AddDays(7),
                    IsRevoked = false
                });

            // Persist changes
            await _unitOfWork.SaveAsync(cancellationToken);

            // Build successful response
            response.Data = new RefreshTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiredAt = _dateTimeProvider.UtcNow.AddMinutes(15)
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);

            _logger.LogInformation(
                "{FunctionName} Refresh token generated successfully for UserId: {UserId}",
                functionName,
                user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{FunctionName} Unexpected error occurred.",
                functionName);

            response.ErrorMessage = ex.Message;
        }

        return response;
    }
}