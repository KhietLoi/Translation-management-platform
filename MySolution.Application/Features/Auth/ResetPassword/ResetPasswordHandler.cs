using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly ILogger<ResetPasswordHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordResetTokenService  _tokenService;

    public ResetPasswordHandler
    (
        ILogger<ResetPasswordHandler> logger,
		IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IPasswordResetTokenService tokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    #region Implementation of IRequestHandler<in ResetPasswordCommand, ResetPasswordResponse>

    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var payload =  request.Payload;
        var functionName = $"{nameof(ResetPasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ResetPasswordResponse();

        try
        {
            var tokenPayload = _tokenService.ValidateToken(payload.Token);
            if (tokenPayload.ExpiredAt < DateTime.UtcNow)
            {
                response.ErrorMessage = "Token expired.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var user = _unitOfWork.User.GetByIdAsync(tokenPayload.UserId).Result;
            if (user == null)
            {
                response.ErrorMessage= "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            //Check passwordversion:
            if (tokenPayload.PasswordVersion != user.PasswordVersion)
            {
                response.ErrorMessage = "Token is invalid.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            //Change Password:
            user.PasswordHash = _passwordHasher.HashPassword(payload.NewPassword);
            user.PasswordVersion++;
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