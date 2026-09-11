using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
using Shared.Extensions;
namespace MySolution.Application.Features.Notification.Queries.GetNotifications;

public class GetNotificationsHandler : IRequestHandler<GetNotificationsQuery, GetNotificationsResponse>
{
    private readonly ILogger<GetNotificationsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetNotificationsHandler
    (
        ILogger<GetNotificationsHandler> logger,
		IUnitOfWork unitOfWork,
        ICurrentUser currentUser
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    #region Implementation of IRequestHandler<in GetNotificationsQuery, GetNotificationsResponse>

    public async Task<GetNotificationsResponse> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetNotificationsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetNotificationsResponse();
        
        
        try
        {
            var result =
                await _unitOfWork.Notification.GetAsync(
                    _currentUser.UserId,
                    request.Payload.ProjectId,
                    request.Payload.IsRead,
                    request.Payload.PageNumber,
                    request.Payload.PageSize,
                    cancellationToken);


            response.Data = new GetNotificationsData
            {
                Notifications = result.Notifications
                    .Select(x => new NotificationItemResponse
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        TriggeredByUserId = x.TriggeredByUserId,
                        TriggeredByUserName = x.TriggeredByUser?.Username ?? string.Empty,
                        ProjectId = x.ProjectId,
                        Title = x.Title,
                        Message = x.Message,
                        Type = x.Type,
                        IsRead = x.IsRead,
                        NavigationUrl = x.NavigationUrl,
                        CreatedAt = x.CreatedAt
                    }).ToList(),
                TotalCount = result.TotalCount,
                PageNumber = request.Payload.PageNumber,
                PageSize = request.Payload.PageSize
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

    #endregion
}