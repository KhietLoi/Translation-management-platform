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
public class SdkController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Get translations for the application based on the specified language.
    /// </summary>
    /// <param name="language"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("projects/translations")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.TranslationRead)]
    public async Task<IActionResult> GetTranslations([FromQuery] string language, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationTranslationsQuery(language), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    ///  Get the current version of the application.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("projects/version")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.VersionRead)]
    public async Task<IActionResult> GetVersion(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationVersionQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get the application package for download.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("projects/package")]
    [ApiKeyAuthorize]
    [ApiKeyPermission(ApiKeyPermissionType.PackageDownload)]
    public async Task<IActionResult> GetPackage(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationPackageQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}