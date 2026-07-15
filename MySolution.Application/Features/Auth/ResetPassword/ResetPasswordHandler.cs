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

    public ResetPasswordHandler
    (
        ILogger<ResetPasswordHandler> logger,
		IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
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
            var token = _unitOfWork.PasswordResetToken.GetByTokenAsync(payload.Token).Result;
            // Check token is null:
            if (token == null)
            {
                response.ErrorMessage = "Invalid token.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // Check token isUsed:
            if (token.IsUsed)
            {
                response.ErrorMessage = "Token is used.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            // CheckToken isExpired
            if (token.IsExpired)
            {
                response.ErrorMessage = "Token is expired.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var user = _unitOfWork.User.GetByIdAsync(token.UserId).Result;
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            user.PasswordHash = _passwordHasher.HashPassword(payload.NewPassword);
            token.IsUsed = true;
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