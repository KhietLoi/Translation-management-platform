using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Notification.Queries.GetNotifications;

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
    
}