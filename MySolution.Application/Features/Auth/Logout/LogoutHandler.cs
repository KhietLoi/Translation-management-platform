using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
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

    public LogoutHandler
    (
        ILogger<LogoutHandler> logger,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
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

            // Revoke all refresh tokens of the current user
            await _unitOfWork.RefreshToken
                .RevokeAsync(_currentUser.UserId);
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