using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.AuditLogs.Queries.GetAuditLogs;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuditLogController (IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string entityName,
        [FromQuery] Guid entityId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)  
    {
        var response  = await mediator.Send(
            new GetAuditLogsQuery
            {
                EntityName = entityName,
                EntityId = entityId,
                PageNumber = pageNumber,
                PageSize = pageSize
            }, cancellationToken);
        
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}