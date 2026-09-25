using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationKey;
using MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationKey;
using MySolution.Application.Features.TranslationManagement.Commands.UpdateTranslationKey;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeyById;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationKeys;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationKeyController (IMediator mediator) : Controller
{
    /// <summary>
    /// Creates a new translation key.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConstants.Translation.Create)]
    public async Task<IActionResult> CreateTranslationKey([FromBody] CreateTranslationKeyRequest request, CancellationToken cancellationToken = default)
    {
        var response =  await mediator.Send(new CreateTranslationKeyCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Updates an existing translation key by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.Translation.Update)]
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

    /// <summary>
    /// Deletes a translation key by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Translation.Delete)]
    public async Task<IActionResult> DeleteTranslationKey(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new DeleteTranslationKeyCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
    /// <summary>
    /// Retrieves translation keys based on optional filters such as project ID, namespace ID, and search term.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="namespaceId"></param>
    /// <param name="search"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationKeys(
        [FromQuery] Guid? projectId,
        [FromQuery] Guid? namespaceId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationKeysQuery(projectId, namespaceId, search), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Retrieves a translation key by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationKeyById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationKeyByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}