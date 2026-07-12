using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.RolePermissions.Commands.AssignPermissionToRole;
using MySolution.Application.Features.RolePermissions.Commands.RemovePermissionFromRole;
using MySolution.Application.Features.Roles.Commands.CreateRole;
using MySolution.Application.Features.Roles.Commands.DeleteRole;
using MySolution.Application.Features.Roles.Commands.UpdateRole;
using MySolution.Application.Features.Roles.Commands.UpdateRolePermissions;
using MySolution.Application.Features.Roles.Queries.GetRoleById;
using MySolution.Application.Features.Roles.Queries.GetRoles;

namespace MySolution.Api.Controllers;
[Route("api/[controller]") ]
[ApiController]
public class RoleController(IMediator mediator) : Controller
{
    /// <summary>
    /// Create a new role
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateRoleCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Update an existing role
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send( new UpdateRoleCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response,response.Data);
    }
    
    /// <summary>
    /// Delete a role by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get all roles with optional filtering and pagination
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send( new GetRolesQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get a role by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Assign a permission to a role
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("assign-permission")]
    public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionToRoleRequest request,CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new AssignPermissionToRoleCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Remove a permission from a role
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="permissionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("delete-permission")]
    public async Task<IActionResult> RemovePermissionFromRole(Guid roleId, Guid permissionId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RemovePermissionFromRoleCommand(roleId, permissionId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    
    [HttpPut("permissions")]
    public async Task<IActionResult> UpdateRolePermissions(
        [FromBody] UpdateRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new UpdateRolePermissionsCommand(request),
            cancellationToken);

        return ResponseHelper.ToResponse(
            response.StatusCode,
            response,
            response.Data);
    }
    
    
}