using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Roles.Commands.CreateRole;
using MySolution.Application.Features.Roles.Commands.DeleteRole;
using MySolution.Application.Features.Roles.Commands.UpdateRole;
using MySolution.Application.Features.Users.Commands.UpdateUser;

namespace MySolution.Api.Controllers;
[Route("api/[controller]") ]
[ApiController]
public class RoleController : Controller
{
    private readonly IMediator _mediator;
    public RoleController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    //Create new Role:
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new CreateRoleCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Update Role
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send( new UpdateRoleCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response,response.Data);
    }
    
    //Delete Role
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new DeleteRoleCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
}