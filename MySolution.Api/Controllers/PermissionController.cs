using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
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
    /// <summary>
    /// Create a new permission
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConstants.Permission.Create)]
    public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request,CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreatePermissionCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Update an existing permission
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.Permission.Update)]
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

    /// <summary>
    /// Delete a permission by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Permission.Delete)]
    public async Task<IActionResult> DeletePermission(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeletePermissionCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get all permissions with optional filtering and pagination
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Permission(PermissionConstants.Permission.View)]
    public async Task<IActionResult> GetPermissions([FromQuery] GetPermissionsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send( new GetPermissionsQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Get a permission by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [Permission(PermissionConstants.Permission.View)]
    public async Task<IActionResult> GetPermissionById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetPermissionByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}