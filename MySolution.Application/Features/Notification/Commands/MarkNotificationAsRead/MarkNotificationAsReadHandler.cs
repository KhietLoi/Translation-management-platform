using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>
{
    private readonly ILogger<MarkNotificationAsReadHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadHandler
    (
        ILogger<MarkNotificationAsReadHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>

    public async Task<MarkNotificationAsReadResponse> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(MarkNotificationAsReadHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new MarkNotificationAsReadResponse();

        try
        {

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