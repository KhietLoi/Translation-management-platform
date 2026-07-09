using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.User.Commands.CreateUser;
using MySolution.Application.Features.User.Commands.DeleteUser;
using MySolution.Application.Features.User.Commands.UpdateUser;
using MySolution.Application.Features.User.Queries.GetUser;
using MySolution.Application.Features.User.Queries.GetUserById;
using MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;
using MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

namespace MySolution.Api.Controllers;

[Route("api/[controller]") ]
[ApiController]
public class UserController(IMediator mediator) : Controller
{
    //Get All User, Limit by page,
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUsersQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data
        );
    }
    
    //Get User by Id:
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
           
        return ResponseHelper.ToResponse(
            response.StatusCode,
            response,
            response.Data);
    }
    
    // Create User
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await  mediator.Send(new CreateUserCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Update User
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateUserCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Delete User
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Assign Role to user
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new AssignRoleToUserCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    //Remove Role to user
    [HttpPut("delete-role")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid roleid, Guid userid, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RemoveRoleFromUserCommand(roleid,userid), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}