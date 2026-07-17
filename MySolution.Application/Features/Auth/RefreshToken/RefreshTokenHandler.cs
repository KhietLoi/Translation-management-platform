using System.Net;
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
    private  readonly IHashService _hashService;
    public RefreshTokenHandler
    (
        ILogger<RefreshTokenHandler> logger,
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IHashService hashService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _hashService =  hashService;
    }
   public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(RefreshTokenHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new RefreshTokenResponse();

        try
        {
            var hash = _hashService.ComputeSha256(payload.RefreshToken);
            var refreshToken = await _unitOfWork.RefreshToken.GetByHashAsync(hash);
            if (refreshToken is null)
            {
                response.ErrorMessage = "Refresh token not found.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }
            
            // Check whether the refresh token has been revoked
            if (!refreshToken.IsActive)
            {
                response.ErrorMessage = "Refresh token has been revoked.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }
            
            var user = await _unitOfWork.User.GetUserWithRolesAsync(refreshToken.UserId);
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
            var newHash = _hashService.ComputeSha256(newRefreshToken);
            // Revoke the current refresh token
            refreshToken.RevokedAt =  DateTime.UtcNow;
      
            // Store the newly generated refresh token
            await _unitOfWork.RefreshToken.Add(
                new Domain.Entities.RefreshToken
                {
                    Id = Guid.CreateVersion7(),
                    UserId = user.Id,
                    TokenHash = newHash,
                    CreatedAt = DateTime.UtcNow,
                    ExpiredAt = _jwtService.GetRefreshTokenExpirationDate()
                });

            // Persist changes
            await _unitOfWork.SaveAsync(cancellationToken);

            // Build successful response
            response.Data = new RefreshTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresAtAccessToken = _jwtService.GetAccessTokenExpirationDate(),
                ExpiresAtRefreshToken = _jwtService.GetRefreshTokenExpirationDate()
            };

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);

            _logger.LogInformation("{FunctionName} Refresh token generated successfully for UserId: {UserId}", functionName, user.Id);
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