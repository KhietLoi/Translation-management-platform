using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;
using Shared.Extensions;

namespace MySolution.Application.Features.Auth.ResetPassword;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    private readonly ILogger<ResetPasswordHandler> _logger;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPasswordResetTokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

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
        var payload = request.Payload;
        var functionName = $"{nameof(ResetPasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ResetPasswordResponse();

        try
        {
            var tokenPayload = _tokenService.ValidateToken(payload.Token);
            if (tokenPayload.ExpiredAt < DateTime.UtcNow)
            {
                _logger.LogInformation($"{functionName} Token expired. UserId: {tokenPayload.UserId}, ExpiredAt: {tokenPayload.ExpiredAt}");
                
                response.ErrorMessage = "Token expired.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var user = await _unitOfWork.User
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == tokenPayload.UserId, cancellationToken);
            
            if (user == null)
            {
                _logger.LogInformation($"{functionName} User not found. UserId: {tokenPayload.UserId}");
                
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            //Check passwordversion:
            if (tokenPayload.PasswordVersion != user.PasswordVersion)
            {
                _logger.LogInformation($"{functionName} Token is invalid. UserId: {tokenPayload.UserId}, TokenPasswordVersion: {tokenPayload.PasswordVersion}, UserPasswordVersion: {user.PasswordVersion}");
                
                response.ErrorMessage = "Token is invalid.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            //Change Password:
            user.PasswordHash = _passwordHasher.HashPassword(payload.NewPassword);
            if (user.Status == UserStatus.NonActive)
            {
                user.Status = UserStatus.Active;
            }
            
            user.PasswordVersion++;
            await _unitOfWork.SaveAsync(cancellationToken);
                
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"{functionName} An unexpected error occurred.");
            
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}