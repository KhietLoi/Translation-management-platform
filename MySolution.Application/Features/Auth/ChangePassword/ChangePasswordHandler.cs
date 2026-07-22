using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly ILogger<ChangePasswordHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUser _currentUser;
    private readonly ISecurityStampService _tokenSecurityService;

    public ChangePasswordHandler
    (
        ILogger<ChangePasswordHandler> logger,
		IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUser currentUser,
        ISecurityStampService tokenSecurityService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
        _tokenSecurityService = tokenSecurityService;
    }

    #region Implementation of IRequestHandler<in ChangePasswordCommand, ChangePasswordResponse>

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ChangePasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ChangePasswordResponse();

        try
        {
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
            
            //Hash new password:
            user.PasswordHash = _passwordHasher.HashPassword(request.Payload.NewPassword);
            //Revoke all refresh Token:
            await _unitOfWork.RefreshToken.RevokeAsync(user.Id);
            //Create new SecurityStamp to invalidate existing tokens:
            user.SecurityStamp = Guid.CreateVersion7().ToString();
            //Save
            await _unitOfWork.SaveAsync(cancellationToken);
            //Update Redis:
            await _tokenSecurityService.SetSecurityStampAsync(user.Id, user.SecurityStamp);
            
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