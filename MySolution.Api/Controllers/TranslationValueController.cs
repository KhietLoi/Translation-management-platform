using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValueById;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValues;
using MySolution.Application.Features.TranslationValue.Commands.CreateTranslationValue;
using MySolution.Application.Features.TranslationValue.Commands.DeleteTranslationValue;
using MySolution.Application.Features.TranslationValue.Commands.UpdateTranslationValue;
using MySolution.Application.Features.TranslationValue.Queries.GetTranslationValues;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationValueController (IMediator mediator) : Controller
{
    [HttpPost]
    [Permission(PermissionConstants.Translation.Create)]
    public async Task<IActionResult> CreateTranslationValue([FromBody] CreateTranslationValueRequest request,
        CancellationToken cancellationToken = default)
    {
        var response =  await mediator.Send(new CreateTranslationValueCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
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
    
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Translation.Delete)]
    public async Task<IActionResult> DeleteTranslationValue(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new DeleteTranslationValueCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
    [HttpGet]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationValues(
        [FromQuery] Guid? translationKeyId,
        [FromQuery] Guid? namespaceId,
        [FromQuery] Guid? languageId,
        [FromQuery] TranslationStatus? status,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationValuesQuery(
                translationKeyId,
                namespaceId,
                languageId,
                status),
            cancellationToken);

        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTranslationValueById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationValueByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}