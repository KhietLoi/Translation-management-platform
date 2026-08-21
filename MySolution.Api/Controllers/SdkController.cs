using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.Application;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Sdk.Queries.GetApplicationPackage;
using MySolution.Application.Features.Sdk.Queries.GetApplicationTranslations;
using MySolution.Application.Features.Sdk.Queries.GetApplicationVersion;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/sdk")]
public class SdkController (IMediator mediator) : ControllerBase
{
    [HttpGet("projects/translations")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.TranslationRead)]
    public async Task<IActionResult> GetTranslations(
        [FromQuery] string language,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new GetApplicationTranslationsQuery
            {
                Language = language
            }, cancellationToken);

        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //Get version:
    [HttpGet("projects/{projectId:guid}/version")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.VersionRead)]
    public async Task<IActionResult> GetVersion(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationVersionQuery { ProjectId = projectId }, cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("projects/{projectId:guid}/package")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.PackageDownload)]
    public async Task<IActionResult> GetPackage(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationPackageQuery{ ProjectId = projectId }, cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

}