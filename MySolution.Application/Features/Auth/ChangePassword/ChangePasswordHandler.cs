using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Auth.ChangePassword;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponse>
{
    private readonly ILogger<ChangePasswordHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordHandler
    (
        ILogger<ChangePasswordHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in ChangePasswordCommand, ChangePasswordResponse>

    public async Task<ChangePasswordResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(ChangePasswordHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new ChangePasswordResponse
        {
            Success = false,
            StatusCode = HttpStatusCode.InternalServerError
        };

        try
        {

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