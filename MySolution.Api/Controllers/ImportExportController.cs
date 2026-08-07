using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.ImportExport.Commands.ExportTranslations;

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
    
}