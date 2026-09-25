using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.ApiKey.Command.AssignApiKeyPermission;
using MySolution.Application.Features.ApiKey.Command.RevokeApiKey;
using MySolution.Application.Features.ApiKey.Command.RotateApiKey;
using MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApiKeyController (IMediator mediator) : Controller
{
    /// <summary>
    /// Assign permissions to an API key
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}/permissions")]
    [Permission(PermissionConstants.Permission.Update)]
    public async Task<IActionResult> AssignPermissions
    (
        Guid id,
        [FromBody] AssignApiKeyPermissionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new AssignApiKeyPermissionCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Rotate an API key
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/rotate")]
    [Permission(PermissionConstants.Permission.Update)]
    public async Task<IActionResult> Rotate(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RotateApiKeyCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result, result.Data);
    }
    
    /// <summary>
    /// Revoke an API key
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}/revoke")]
    [Permission(PermissionConstants.Permission.Update)]
    public async Task<IActionResult> Revoke(Guid id,  CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RevokeApiKeyCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result);
    }
    
    /// <summary>
    /// Get API key dashboard grid
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("Grid")]
    [Permission(PermissionConstants.Permission.View)]
    public async Task<IActionResult> GetGrid([FromQuery] GetApiKeyGridRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetApiKeyGridQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result, result.Data);
    }
}