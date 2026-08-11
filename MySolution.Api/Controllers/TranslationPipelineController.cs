using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.ImportExport.Commands.ExportTranslations;
using MySolution.Application.Features.TranslationPipeline.Commands.ImportTranslations;
using MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;
using MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseHistory;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslationPipelineController(IMediator mediator) : ControllerBase
{
    [HttpPost("import")]
    [Permission(PermissionConstants.Translation.Create)]
    public async Task<IActionResult> Import([FromForm] ImportTranslationsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ImportTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPost("export")]
    [Permission(PermissionConstants.Translation.Create)]
    public async Task<IActionResult> Export([FromBody] ExportTranslationsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ExportTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPost("publish")]
    [Authorize]
    public async Task<IActionResult> Publish([FromBody] PublishTranslationsRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new PublishTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("release-history")]
    public async Task<IActionResult> GetReleaseHistory(
        [FromQuery] GetReleaseHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    
}
