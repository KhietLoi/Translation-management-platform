using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Authorization.User;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.TranslationManagement.Commands.BatchReviewTranslation;
using MySolution.Application.Features.TranslationManagement.Commands.BatchUpdateTranslation;
using MySolution.Application.Features.TranslationManagement.Commands.RejectTranslation;
using MySolution.Application.Features.TranslationManagement.Commands.ReviewTranslation;
using MySolution.Application.Features.TranslationManagement.Commands.SubmitTranslation;
using MySolution.Application.Features.TranslationManagement.Queries.GetBatchTranslationSuggestion;
using MySolution.Application.Features.TranslationManagement.Queries.GetPendingLanguageCounts;
using MySolution.Application.Features.TranslationManagement.Queries.GetPendingNamespaceCounts;
using MySolution.Application.Features.TranslationManagement.Queries.GetReviewTranslations;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationGrid;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationSuggestion;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationValuesForBatch;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationManagementController(IMediator mediator) : Controller
{
    /// <summary>
    /// Get translation grid with optional filters for project, namespace, keyword, status, and pagination.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="namespaceId"></param>
    /// <param name="keyword"></param>
    /// <param name="status"></param>
    /// <param name="numberOfLanguages"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("grid")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetTranslationGrid(
        [FromQuery] Guid projectId,
        [FromQuery] Guid? namespaceId,
        [FromQuery] string? keyword,
        [FromQuery] TranslationStatus? status,
        [FromQuery] int numberOfLanguages = 3,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = await mediator.Send(
            new GetTranslationGridQuery
            {
                ProjectId = projectId,
                NamespaceId = namespaceId,
                Keyword = keyword,
                Status = status,
                NumberOfLanguages = numberOfLanguages,
                PageNumber = pageNumber,
                PageSize = pageSize
            });

        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Reject a translation by its ID with the provided reason in the request body.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/reject")]
    [Permission(PermissionConstants.Translation.Review)]
    public async Task<IActionResult> RejectTranslation(Guid id, [FromBody] RejectTranslationRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RejectTranslationCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Review a translation by its ID, marking it as reviewed.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/review")]
    [Permission(PermissionConstants.Translation.Review)]
    public async Task<IActionResult> ReviewTranslation(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ReviewTranslationCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Submit a translation by its ID, marking it as submitted for review.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id:guid}/submit")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> SubmitTranslation(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new SubmitTranslationCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Get translations pending review with optional filters for project, namespace, keyword, status, and pagination.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("review")]
    [Permission(PermissionConstants.Translation.Review)]
    public async Task<IActionResult> GetReviewTranslations(
        [FromQuery] GetReviewTranslationsRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetReviewTranslationsQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Update the review status of multiple translations.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("batch-review")]
    [Permission(PermissionConstants.Translation.Review)]
    public async Task<IActionResult> BatchReviewTranslations([FromBody] BatchReviewTranslationRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new BatchReviewTranslationCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get translation values for batch update.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("update")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> GetUpdateTranslation([FromQuery] GetTranslationValuesForBatchRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetTranslationValuesForBatchQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Update multiple translations in a batch.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("batch-update")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> BatchUpdateTranslations([FromBody] BatchUpdateTranslationRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new BatchUpdateTranslationCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get translation suggestion for a specific translation value ID.
    /// </summary>
    /// <param name="translationValueId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{translationValueId:guid}/suggest")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> GetTranslationSuggestion([FromRoute] Guid translationValueId, CancellationToken cancellationToken)
    {
        var request = new GetTranslationSuggestionRequest { TranslationValueId = translationValueId };
        var response = await mediator.Send(new GetTranslationSuggestionQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Get batch translation suggestions for multiple translation value IDs.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("batch-suggest")]
    [Permission(PermissionConstants.Translation.Update)]
    public async Task<IActionResult> BatchSuggestTranslations
    (
        [FromBody] GetBatchTranslationSuggestionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetBatchTranslationSuggestionQuery(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    /// <summary>
    /// Get pending translation counts by namespace.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("pending-counts/namespaces")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetPendingNamespaceCounts
    (
        [FromQuery] Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetPendingNamespaceCountsQuery(projectId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    /// <summary>
    /// Get pending translation counts by language within namespace.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="namespaceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("pending-counts/languages")]
    [Permission(PermissionConstants.Translation.View)]
    public async Task<IActionResult> GetPendingLanguageCounts
    (
        [FromQuery] Guid projectId,
        [FromQuery] Guid namespaceId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetPendingLanguageCountsQuery(projectId, namespaceId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}