using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Language.Commands.CreateLanguage;
using MySolution.Application.Features.Language.Commands.DeleteLanguage;
using MySolution.Application.Features.Language.Commands.UpdateLanguage;
using MySolution.Application.Features.Language.Queries.GetLanguages;
using MySolution.Application.Features.Project.Commands.CreateProject;
using MySolution.Application.Features.Project.Queries.GetProjects;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LanguageController (IMediator mediator) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateLanguageCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] UpdateLanguageRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateLanguageCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLanguage(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteLanguageCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetLanguages(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetLanguagesQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}