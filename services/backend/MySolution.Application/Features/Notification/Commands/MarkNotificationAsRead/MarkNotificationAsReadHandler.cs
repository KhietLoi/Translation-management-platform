using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;

public class MarkNotificationAsReadHandler : IRequestHandler<MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>
{
    private readonly ILogger<MarkNotificationAsReadHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public MarkNotificationAsReadHandler
    (
        ILogger<MarkNotificationAsReadHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in MarkNotificationAsReadCommand, MarkNotificationAsReadResponse>

    public async Task<MarkNotificationAsReadResponse> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;
        var functionName = $"{nameof(MarkNotificationAsReadHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new MarkNotificationAsReadResponse();

        try
        {
            var notification = await _unitOfWork.Notification
                .GetUserNotificationAsync(payload.NotificationId, _currentUser.UserId,cancellationToken);
            if (notification == null)
            {
                response.ErrorMessage = "Notification not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            notification.IsRead = true;
            await _unitOfWork.SaveAsync(cancellationToken);
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