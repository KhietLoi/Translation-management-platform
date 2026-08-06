using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.ApiKey.Command.AssignApiKeyPermission;
using MySolution.Application.Features.ApiKey.Command.RevokeApiKey;
using MySolution.Application.Features.ApiKey.Command.RotateApiKey;
using MySolution.Application.Features.ApiKey.Queries.GetApiKeyGrid;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api-keys")]
public class ApiKeyController (IMediator mediator) : Controller
{
    [HttpPut("{id:guid}/permissions")]
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
    
    [HttpPost("{id:guid}/rotate")]
    public async Task<IActionResult> Rotate(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RotateApiKeyCommand(id), cancellationToken);

        return ResponseHelper.ToResponse(result.StatusCode, result, result.Data);
    }
    
    [HttpPut("{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id,  CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RevokeApiKeyCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result);
    }
    
    [HttpGet("Grid")]
    public async Task<IActionResult> GetGrid([FromQuery] GetApiKeyGridRequest request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetApiKeyGridQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(result.StatusCode, result, result.Data);
    }

}