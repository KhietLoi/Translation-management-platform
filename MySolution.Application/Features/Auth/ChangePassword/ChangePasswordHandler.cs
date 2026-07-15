using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly ILogger<ChangePasswordHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUser _currentUser;

    public ChangePasswordHandler
    (
        ILogger<ChangePasswordHandler> logger,
		IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in ChangePasswordCommand, ChangePasswordResponse>

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ChangePasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ChangePasswordResponse();

        try
        {
            if (!_currentUser.IsAuthenticated)
            {
                response.ErrorMessage = "Unauthorized.";
                response.WithStatus(HttpStatusCode.Unauthorized);
                return response;
            }
            
            var user = await _unitOfWork.User.GetByIdAsync(_currentUser.UserId);
            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Verify current password:
            var isCurrentPasswordValid = _passwordHasher.VerifyPassword(request.Payload.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                response.ErrorMessage = "Current password is incorrect.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            //Check newPassword is same current password:
            var isSamePassword = _passwordHasher.VerifyPassword(request.Payload.NewPassword, user.PasswordHash);
            if (isSamePassword)
            {
                response.ErrorMessage = "New password must be different from current password.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            //Hash new password:
            user.PasswordHash = _passwordHasher.HashPassword(request.Payload.NewPassword);
            
            //Save
            await _unitOfWork.SaveAsync(cancellationToken);
            
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception exception)
        {
            exception.LogError(_logger, functionName);
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }
    #endregion
}