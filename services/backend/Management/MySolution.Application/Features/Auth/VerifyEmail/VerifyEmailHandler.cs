using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Repositories;

namespace MySolution.Application.Features.Auth.VerifyEmail;

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResponse>
{
    private readonly IEmailVerificationTokenService _emailVerificationTokenService;

    private readonly ILogger<VerifyEmailHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailHandler
    (
        ILogger<VerifyEmailHandler> logger,
        IUnitOfWork unitOfWork,
        IEmailVerificationTokenService emailVerificationTokenService
    )
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _emailVerificationTokenService = emailVerificationTokenService;
    }

    public async Task<VerifyEmailResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(VerifyEmailHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new VerifyEmailResponse();
        var payload = _emailVerificationTokenService.ValidateToken(request.Token);

        try
        {
            var user = await _unitOfWork.User
                .GetAll()
                .FirstOrDefaultAsync(u => u.Id == payload.UserId, cancellationToken);
            if (user == null)
            {
                _logger.LogInformation("{FunctionName} User not found for userId: {UserId}", functionName, payload.UserId);
                
                response.ErrorMessage = "User not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            if (payload.ExpiredAt < DateTime.UtcNow)
            {
                _logger.LogInformation("{FunctionName} Verification token is expired for userId: {UserId}", functionName, payload.UserId);
                
                response.ErrorMessage = "Verification token is expired.";
                response.Data = new VerifyEmailData
                {
                    Email = user.Email
                };
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            if (user.IsEmailVerified)
            {
                _logger.LogInformation("{FunctionName} Email is already verified for userId: {UserId}", functionName, payload.UserId);
                
                response.ErrorMessage = "Email is already verified.";
                response.Data = new VerifyEmailData
                {
                    Email = user.Email
                }; 
                response.WithStatus(HttpStatusCode.BadRequest);
                return response;
            }

            user.IsEmailVerified = true;
            await _unitOfWork.SaveAsync(cancellationToken);

            response.Data = new VerifyEmailData
            {
                Email = user.Email
            };
            
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
}