using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.TranslationManagement.Queries.GetTranslationGrid;
using MySolution.Domain.Enums;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TranslationManagementController (IMediator mediator) : Controller
{
    [HttpGet("grid")]
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

}