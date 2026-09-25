using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.Admin.Roles.Commands.CreateRole;
using MySolution.Application.Features.Admin.Roles.Commands.DeleteRole;
using MySolution.Application.Features.Admin.Roles.Commands.UpdateRole;
using MySolution.Application.Features.Admin.Roles.Commands.UpdateRolePermissions;
using MySolution.Application.Features.Admin.Roles.Queries.GetRoleById;
using MySolution.Application.Features.Admin.Roles.Queries.GetRoles;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController(IMediator mediator) : Controller
{
    /// <summary>
    /// Creates a new role with the specified details.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConstants.Role.Create)]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request,CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateRoleCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Updates the details of an existing role identified by the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.Role.Update)]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateRoleCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Deletes an existing role identified by the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Role.Delete)]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Retrieves a list of roles based on the provided query parameters.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Permission(PermissionConstants.Role.View)]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetRolesQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Retrieves the details of a specific role identified by the provided ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [Permission(PermissionConstants.Role.View)]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Updates the permissions associated with a specific role identified by the provided ID.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("permissions")]
    [Permission(PermissionConstants.Role.Update)]
    public async Task<IActionResult> UpdateRolePermissions([FromBody] UpdateRolePermissionsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateRolePermissionsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}