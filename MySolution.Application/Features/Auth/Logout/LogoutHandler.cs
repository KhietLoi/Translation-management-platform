using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.Logout;

/// <summary>
/// Handler for processing logout requests.
/// It revokes all refresh tokens associated with the current user and returns a response indicating the success or failure of the operation.
/// </summary>
public class LogoutHandler : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private readonly ILogger<LogoutHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ITokenBlacklistService _tokenBlacklistService;
    public LogoutHandler
    (
        ILogger<LogoutHandler> logger,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ITokenBlacklistService tokenBlacklistService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _tokenBlacklistService = tokenBlacklistService;
    }
    public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(LogoutHandler)}";
        _logger.LogInformation(functionName);
        var response = new LogoutResponse();

        try
        {
            
            // Check if the user is authenticated
            if (!_currentUser.IsAuthenticated)
            {
                response.ErrorMessage = "Unauthorized.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }
            
            //_logger.LogInformation("JTI = {Jti}", _currentUser.Jti);
            //_logger.LogInformation("ExpiredAt = {ExpiredAt}", _currentUser.ExpiredAt);
            //Check jti:
            if (!string.IsNullOrWhiteSpace(_currentUser.Jti) && _currentUser.ExpiredAt.HasValue)
            {
                var ttl = _currentUser.ExpiredAt.Value - DateTime.UtcNow;
                //_logger.LogInformation("TTL = {TTL}", ttl);
                if (ttl > TimeSpan.Zero)
                {
                    await _tokenBlacklistService.BlacklistAsync(_currentUser.Jti, ttl);
                }
            }
            
            // Revoke all refresh tokens of the current user
            await _unitOfWork.RefreshToken.RevokeAsync(_currentUser.UserId);
            // Save changes
            await _unitOfWork.SaveAsync(cancellationToken);
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
            _logger.LogInformation("{FunctionName} User {UserId} logged out successfully.", functionName, _currentUser.UserId);
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