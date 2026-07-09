using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Permission.Commands.CreatePermission;
using MySolution.Application.Features.Permission.Commands.DeletePermission;
using MySolution.Application.Features.Permission.Commands.UpdatePermission;
using MySolution.Application.Features.Permission.Queries.GetPermissionById;
using MySolution.Application.Features.Permission.Queries.GetPermissions;
using MySolution.Application.Features.Roles.Queries.GetRoles;
using MySolution.Domain.Entities;

namespace MySolution.Api.Controllers;

[Route("api/[controller]") ]
[ApiController]
public class PermissionController (IMediator mediator) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request,CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreatePermissionCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePermission
    (
        Guid id,
        [FromBody] UpdatePermissionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdatePermissionCommand(id,request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePermission(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeletePermissionCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPermissions([FromQuery] GetPermissionsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send( new GetPermissionsQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPermissionById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetPermissionByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    
    
}