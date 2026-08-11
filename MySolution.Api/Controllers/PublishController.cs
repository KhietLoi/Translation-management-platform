using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PublishController (IMediator mediator) : Controller
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Publish([FromBody] PublishTranslationsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new PublishTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}