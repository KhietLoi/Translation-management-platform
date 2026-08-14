using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Notification.Commands.MarkAllNotificationAsRead;
using MySolution.Application.Features.Notification.Commands.MarkNotificationAsRead;
using MySolution.Application.Features.Notification.Queries.GetNotifications;
using MySolution.Application.Features.Notification.Queries.GetUnreadCount;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController (IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] GetNotificationsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetNotificationsQuery(request), cancellationToken);

        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("unread-count")]
    [Authorize]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUnreadCountQuery(new GetUnreadCountRequest()), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{notificationId:guid}/read")]
    [Authorize]
    public async Task<IActionResult> MarkAsRead(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new MarkNotificationAsReadCommand(new MarkNotificationAsReadRequest { NotificationId = notificationId }), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }

    [HttpPut("read-all")]
    [Authorize]
    public async Task<IActionResult> MarkAllAsRead(
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new MarkAllNotificationAsReadCommand(new MarkAllNotificationAsReadRequest()), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
}