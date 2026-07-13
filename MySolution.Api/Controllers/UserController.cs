using MediatR;
/*using Microsoft.AspNetCore.Authorization;*/
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.User.Commands.CreateUser;
using MySolution.Application.Features.User.Commands.DeleteUser;
using MySolution.Application.Features.User.Commands.UpdateUser;
using MySolution.Application.Features.User.Commands.UpdateUserRoles;
using MySolution.Application.Features.User.Queries.GetUser;
using MySolution.Application.Features.User.Queries.GetUserById;
using MySolution.Application.Features.UserRoles.Commands.AssignRoleToUser;
using MySolution.Application.Features.UserRoles.Commands.RemoveRoleToUser;

namespace MySolution.Api.Controllers;

[Route("api/[controller]") ]
[ApiController]
public class UserController(IMediator mediator) : Controller
{
    /// <summary>
    /// Get all users with optional filtering and pagination
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Permission(PermissionConstants.User.View)]
    public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUsersQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data
        );
    }
    
    /// <summary>
    /// Get a user by their ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [Permission(PermissionConstants.User.View)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConstants.User.Create)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await  mediator.Send(new CreateUserCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Update an existing user
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.User.Update)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateUserCommand(id, request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Delete a user by their ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.User.Delete)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Assign a role to a user
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("assign-role")]
    [Permission(PermissionConstants.User.Create)]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new AssignRoleToUserCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Remove a role from a user
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("delete-role")]
    [Permission(PermissionConstants.User.Delete)]
    public async Task<IActionResult> RemoveRoleFromUser(Guid roleId, Guid userId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RemoveRoleFromUserCommand(roleId, userId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("roles")]
    [Permission(PermissionConstants.User.Update)]
    public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateUserRolesCommand(request), cancellationToken);
        return ResponseHelper.ToResponse( response.StatusCode, response, response.Data);
    }
}