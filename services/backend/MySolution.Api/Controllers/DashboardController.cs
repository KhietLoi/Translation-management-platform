using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Dashboard.Queries.GetDashboardValue;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController (IMediator mediator) : Controller
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetDashboardValueQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}