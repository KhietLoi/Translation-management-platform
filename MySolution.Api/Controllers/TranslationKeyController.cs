using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.TranslationKey.Commands.CreateTranslationKey;
using MySolution.Application.Features.TranslationKey.Commands.DeleteTranslationKey;
using MySolution.Application.Features.TranslationKey.Commands.UpdateTranslationKey;
using MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeyById;
using MySolution.Application.Features.TranslationKey.Queries.GetTranslationKeys;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationKeyController (IMediator mediator) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateTranslationKey([FromBody] CreateTranslationKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        var response =  await mediator.Send(new CreateTranslationKeyCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTranslationKey
    (
        Guid id,
        [FromBody] UpdateTranslationKeyRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdateTranslationKeyCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTranslationKey(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new DeleteTranslationKeyCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTranslationKeys(
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? namespaceId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationKeysQuery(projectId, namespaceId, search), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTranslationKeyById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationKeyByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}