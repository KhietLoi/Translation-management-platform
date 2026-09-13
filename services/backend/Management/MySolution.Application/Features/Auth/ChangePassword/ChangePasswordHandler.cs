using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<ChangePasswordHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ISecurityStampService _tokenSecurityService;
    private readonly IUnitOfWork _unitOfWork;

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
            var user = await _unitOfWork.User
                .GetAll()
                .FirstOrDefaultAsync(x => x.Id == _currentUser.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogInformation("{FunctionName} User not found.", functionName);
                
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            //Verify current password:
            var isCurrentPasswordValid = _passwordHasher.VerifyPassword(request.Payload.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
            {
                _logger.LogInformation("{FunctionName} Current password is incorrect.", functionName);
                
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}