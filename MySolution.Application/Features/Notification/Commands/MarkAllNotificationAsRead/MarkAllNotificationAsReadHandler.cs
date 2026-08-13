using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Notification.Commands.MarkAllNotificationAsRead;

public class MarkAllNotificationAsReadHandler : IRequestHandler<MarkAllNotificationAsReadCommand, MarkAllNotificationAsReadResponse>
{
    private readonly ILogger<MarkAllNotificationAsReadHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public MarkAllNotificationAsReadHandler
    (
        ILogger<MarkAllNotificationAsReadHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in MarkAllNotificationAsReadCommand, MarkAllNotificationAsReadResponse>

    public async Task<MarkAllNotificationAsReadResponse> Handle(MarkAllNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(MarkAllNotificationAsReadHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new MarkAllNotificationAsReadResponse();

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