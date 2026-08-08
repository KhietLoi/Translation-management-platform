using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.ImportExport.Commands.ExportTranslations;
using MySolution.Application.Features.ImportExport.Commands.ImportTranslations;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportExportController (IMediator mediator) : Controller
{
    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ExportTranslationsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ExportTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPost("import")]
    public async Task<IActionResult> ImportTranslations([FromForm] ImportTranslationsRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ImportTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}