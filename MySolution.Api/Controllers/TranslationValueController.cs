using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.TranslationValue.Commands.CreateTranslationValue;
using MySolution.Application.Features.TranslationValue.Commands.DeleteTranslationValue;
using MySolution.Application.Features.TranslationValue.Commands.UpdateTranslationValue;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationValueController (IMediator mediator) : Controller
{
    
    [HttpPost]
    public async Task<IActionResult> CreateTranslationValue([FromBody] CreateTranslationValueRequest request,
        CancellationToken cancellationToken = default)
    {
        var response =  await mediator.Send(new CreateTranslationValueCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("{id:guid}")]
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
    public async Task<IActionResult> DeleteTranslationValue(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(new DeleteTranslationValueCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
}