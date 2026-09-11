using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.MassTransit;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Application.Features.Auth.ResendVerificationEmail;

public class ResendVerificationEmailHandler : IRequestHandler<ResendVerificationEmailCommand, ResendVerificationEmailResponse>
{
    private readonly ILogger<ResendVerificationEmailHandler> _logger;
    private readonly IMessageSender _messageSender;
    private readonly IEmailVerificationTokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public ResendVerificationEmailHandler
    (
        IUnitOfWork unitOfWork,
        ILogger<ResendVerificationEmailHandler> logger,
        IEmailVerificationTokenService tokenService,
        IMessageSender messageSender
    )
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _tokenService = tokenService;
        _messageSender = messageSender;
    }

    public async Task<ResendVerificationEmailResponse> Handle(ResendVerificationEmailCommand request, CancellationToken cancellationToken)
    {
        var response = new ResendVerificationEmailResponse();

        try
        {
            var user = await _unitOfWork.User.GetByEmailAsync(request.Payload.Email);
            if (user is null)
            {
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            if (user.IsEmailVerified)
            {
                response.ErrorMessage = "Email already verified.";
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            var token = _tokenService.GenerateVerificationToken(user.Id, user.Email);
            await _messageSender.SendMessage<SendVerifyEmailEvent>(
                new SendVerifyEmailEvent
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Token = token
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
}