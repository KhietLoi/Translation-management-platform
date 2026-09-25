using MediatR;
using Microsoft.AspNetCore.Mvc;

using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.Language.Commands.CreateLanguage;
using MySolution.Application.Features.Language.Commands.DeleteLanguage;
using MySolution.Application.Features.Language.Commands.UpdateLanguage;
using MySolution.Application.Features.Language.Queries.GetLanguages;


namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LanguageController (IMediator mediator) : Controller
{
    /// <summary>
    /// Create a new language
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Permission(PermissionConstants.Language.Create)]
    public async Task<IActionResult> CreateLanguage([FromBody] CreateLanguageRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateLanguageCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Update an existing language
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.Language.Update)]
    public async Task<IActionResult> UpdateLanguage(Guid id, [FromBody] UpdateLanguageRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateLanguageCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Delete a language by ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Language.Delete)]
    public async Task<IActionResult> DeleteLanguage(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteLanguageCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Get a list of all languages
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [Permission(PermissionConstants.Language.View)]
    public async Task<IActionResult> GetLanguages(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetLanguagesQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}