using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Authentication;
using MySolution.Application.Common.Interfaces.Repositories;
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
            var query = _unitOfWork.Notification
                .GetAll()
                .AsNoTracking()
                .Where(x => x.UserId == _currentUser.UserId);

            if (request.Payload.ProjectId.HasValue)
            {
                query = query.Where(x =>
                    x.ProjectId == request.Payload.ProjectId.Value);
            }

            if (request.Payload.IsRead.HasValue)
            {
                query = query.Where(x =>
                    x.IsRead == request.Payload.IsRead.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var notifications = await query
                .Include(x => x.TriggeredByUser)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((request.Payload.PageNumber - 1) * request.Payload.PageSize)
                .Take(request.Payload.PageSize)
                .ToListAsync(cancellationToken);
            
            response.Data = new GetNotificationsData
            {
                Notifications = notifications
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
                
                TotalCount = totalCount,
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