using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Notification.Commands.MarkAllNotificationAsRead;
using MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;
using MySolution.Application.Features.Notification.Queries.GetNotificationDetail;
using MySolution.Application.Features.Notification.Queries.GetNotifications;
using MySolution.Application.Features.Notification.Queries.GetUnreadCount;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController (IMediator mediator) : Controller
{
    /// <summary>
    /// Get notifications for the current user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetNotificationsQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get unread notification count for the current user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUnreadCountQuery(new GetUnreadCountRequest()), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{notificationId:guid}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new MarkNotificationAsReadCommand(new MarkNotificationAsReadRequest { NotificationId = notificationId }), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new MarkAllNotificationAsReadCommand(new MarkAllNotificationAsReadRequest()), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{notificationId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetNotificationDetail(Guid notificationId, CancellationToken cancellationToken)
    {
        var response =
            await mediator.Send(
                new GetNotificationDetailQuery(new GetNotificationDetailRequest { NotificationId = notificationId }),
                cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}