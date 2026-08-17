using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.Application;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Sdk.Queries.GetApplicationTranslations;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/sdk")]
public class SdkController (IMediator mediator) : ControllerBase
{
    [HttpGet("projects/{projectId:guid}/translations")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.TranslationRead)]
    public async Task<IActionResult> GetTranslations(
        Guid projectId,
        [FromQuery] string language,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetApplicationTranslationsQuery
            {
                ProjectId = projectId,
                Language = language
            }, cancellationToken);

        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    
}