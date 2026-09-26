using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;

using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Enums;
using Shared.MassTransit.Contracts;
using Shared.MassTransit.Core;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Admin.User.Commands.ResendSetupPassword;

public class ResendSetupPasswordHandler : IRequestHandler<ResendSetupPasswordCommand, ResendSetupPasswordResponse>
{
    private readonly ILogger<ResendSetupPasswordHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ISendEndpointCustomProvider _messageSender;
    private readonly IPasswordResetTokenService  _passwordResetTokenService;

    public ResendSetupPasswordHandler
    (
        ILogger<ResendSetupPasswordHandler> logger,
		IUnitOfWork unitOfWork,
        ISendEndpointCustomProvider messageSender,
        IPasswordResetTokenService passwordResetTokenService
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _messageSender = messageSender;
        _passwordResetTokenService = passwordResetTokenService;
    }

    #region Implementation of IRequestHandler<in ResendSetupPasswordCommand, ResendSetupPasswordResponse>

    public async Task<ResendSetupPasswordResponse> Handle(ResendSetupPasswordCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ResendSetupPasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ResendSetupPasswordResponse();

        try
        {
            var user = await _unitOfWork.User
                .GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == request.Payload.Email, cancellationToken);
            if (user == null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (user.Status == UserStatus.Active)
            {
                response.ErrorMessage = "User has already activated the account.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }
            
            var token = _passwordResetTokenService.GenerateResetToken(user.Id,user.Email,user.Username, user.PasswordVersion);

            var sendSetUpPasswordEmailEvent = new SendSetUpPasswordEmailEvent
            {
                UserId = user.Id,
                Username = user.Email,
                Email = user.Email,
                Token = token
            };
            
            await _messageSender.SendMessage<SendSetUpPasswordEmail>(sendSetUpPasswordEmailEvent, cancellationToken);
            
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