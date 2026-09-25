using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.MassTransit.Core;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.ForgotPassword;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private readonly ILogger<ForgotPasswordHandler> _logger;
    private readonly ISendEndpointCustomProvider _messageSender;
    private readonly IPasswordResetTokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordHandler
    (
        ILogger<ForgotPasswordHandler> logger,
        IUnitOfWork unitOfWork,
        IPasswordResetTokenService tokenService,
        ISendEndpointCustomProvider messageSender
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _messageSender = messageSender;
    }

    #region Implementation of IRequestHandler<in ForgotPasswordCommand, ForgotPasswordResponse>

    public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(ForgotPasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ForgotPasswordResponse();

        try
        {
            var user = await _unitOfWork.User
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == payload.Email, cancellationToken);
            
            if (user == null)
            {
                response.ErrorMessage = "User not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var resetToken = _tokenService.GenerateResetToken(user.Id, user.Email, user.Username, user.PasswordVersion);    
                
            //SendEmail
            await _messageSender.SendMessage<SendForgotPasswordEmailEvent>(
                new SendForgotPasswordEmailEvent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    Token = resetToken
                }, cancellationToken);

            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ResendVerificationEmail failed");
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}