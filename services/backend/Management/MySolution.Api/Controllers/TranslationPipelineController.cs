using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.TranslationPipeline.Commands.ExportTranslations;
using MySolution.Application.Features.TranslationPipeline.Commands.ImportTranslations;
using MySolution.Application.Features.TranslationPipeline.Commands.PublishTranslations;
using MySolution.Application.Features.TranslationPipeline.Commands.RollbackRelease;
using MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseDiff;
using MySolution.Application.Features.TranslationPipeline.Queries.GetReleaseHistory;
using MySolution.Application.Features.TranslationPipeline.Queries.GetTranslationJob;

namespace MySolution.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TranslationPipelineController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Import translations from a file
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [Permission(PermissionConstants.Translation.Create)]
    public async Task<IActionResult> Import
    (
        [FromForm] ImportTranslationsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new ImportTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Export translations to a file
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("export")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> Export
    (
        [FromBody] ExportTranslationsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new ExportTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Publish translations to a release
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("publish")]
    [Permission(PermissionConstants.Translation.Publish)]
    public async Task<IActionResult> Publish
    (
        [FromBody] PublishTranslationsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new PublishTranslationsCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get release history
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("release-history")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetReleaseHistory
    (
        [FromQuery] GetReleaseHistoryQuery query,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Get translation job history
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("translations-history")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationHistory
    (
        [FromQuery] GetTranslationJobQuery query,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Rollback a release
    /// </summary>
    /// <param name="releaseId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("releases/{releaseId:guid}/rollback")]
    [Permission(PermissionConstants.Translation.Publish)]
    public async Task<IActionResult> RollbackRelease(Guid releaseId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RollbackReleaseCommand(releaseId),cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get the difference between the current release and a target release
    /// </summary>
    /// <param name="targetReleaseId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("release-diff")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetReleaseDiff
    (
        Guid targetReleaseId,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetReleaseDiffQuery(targetReleaseId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}
