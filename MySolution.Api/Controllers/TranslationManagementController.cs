using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.TranslationManagement.Commands.RejectTranslation;
using MySolution.Application.Features.TranslationManagement.Commands.ReviewTranslation;
using MySolution.Application.Features.TranslationManagement.Commands.SubmitTranslation;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationGrid;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationManagementController (IMediator mediator) : Controller
{
    [HttpGet("grid")]
    [Authorize]
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
    
    [HttpPost("{id:guid}/reject")]
    [Authorize]
    public async Task<IActionResult> RejectTranslation(Guid id,[FromBody] RejectTranslationRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new RejectTranslationCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPost("{id:guid}/review")]
    [Authorize]
    public async Task<IActionResult> ReviewTranslation(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ReviewTranslationCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPost("{id:guid}/submit")]
    [Authorize]
    public async Task<IActionResult> SubmitTranslation(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new SubmitTranslationCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}