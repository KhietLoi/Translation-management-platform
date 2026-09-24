using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.TranslationManagement.Commands.CreateTranslationValue;
using MySolution.Application.Features.TranslationManagement.Commands.DeleteTranslationValue;
using MySolution.Application.Features.TranslationManagement.Commands.UpdateTranslationValue;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValueById;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValues;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationValueController (IMediator mediator) : Controller
{
    /// <summary>
    /// Create a new translation value
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> CreateTranslationValue([FromBody] CreateTranslationValueRequest request, CancellationToken cancellationToken = default)
    {
        var response =  await mediator.Send(new CreateTranslationValueCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Update an existing translation value
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> UpdateTranslationValue
    (
        Guid id,
        [FromBody] UpdateTranslationValueRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdateTranslationValueCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Delete a translation value by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> DeleteTranslationValue(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteTranslationValueCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
    /// <summary>
    /// Get translation values based on filters
    /// </summary>
    /// <param name="translationKeyId"></param>
    /// <param name="namespaceId"></param>
    /// <param name="languageId"></param>
    /// <param name="status"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationValues(
        [FromQuery] Guid? translationKeyId,
        [FromQuery] Guid? namespaceId,
        [FromQuery] Guid? languageId,
        [FromQuery] TranslationStatus? status,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationValuesQuery(translationKeyId, namespaceId, languageId, status), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get a translation value by its ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationValueById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationValueByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}