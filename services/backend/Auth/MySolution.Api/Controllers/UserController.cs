using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.Admin.User.Commands.CreateUser;
using MySolution.Application.Features.Admin.User.Commands.DeleteUser;
using MySolution.Application.Features.Admin.User.Commands.ResendSetupPassword;
using MySolution.Application.Features.Admin.User.Commands.UpdateUser;
using MySolution.Application.Features.Admin.User.Commands.UpdateUserRoles;
using MySolution.Application.Features.Admin.User.Queries.GetUser;
using MySolution.Application.Features.Admin.User.Queries.GetUserById;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IMediator mediator) : Controller
{
    [HttpGet]
    [Permission(PermissionConstants.User.View)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUsersQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("{id:guid}")]
    [Permission(PermissionConstants.User.View)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPost]
    [Permission(PermissionConstants.User.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateUserCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateUserCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("roles")]
    [Permission(PermissionConstants.Role.Update)]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateUserRolesCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPost("resend-setup-password")]
    public async Task<IActionResult> ResendSetupPassword([FromBody] ResendSetupPasswordRequest request , CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ResendSetupPasswordCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
}