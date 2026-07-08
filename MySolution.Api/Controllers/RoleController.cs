using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Roles.Commands.CreateRole;
using MySolution.Application.Features.Roles.Commands.DeleteRole;
using MySolution.Application.Features.Roles.Commands.UpdateRole;
using MySolution.Application.Features.Roles.Queries.GetRoleById;
using MySolution.Application.Features.Roles.Queries.GetRoles;

namespace MySolution.Api.Controllers;
[Route("api/[controller]") ]
[ApiController]
public class RoleController(IMediator mediator) : Controller
{
    //Create new Role:
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateRoleCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Update Role
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send( new UpdateRoleCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response,response.Data);
    }
    
    //Delete Role
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    // Get all Role
    [HttpGet]
    public async Task<IActionResult> GetRoles([FromQuery] GetRolesRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send( new GetRolesQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Get Role by id:
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoleById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetRoleByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}