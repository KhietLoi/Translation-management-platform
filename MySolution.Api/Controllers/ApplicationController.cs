
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MySolution.Api.Helpers;
using MySolution.Application.Features.ApiKey.Command.GenerateApiKey;
using MySolution.Application.Features.Application.Command.CreateApplication;
using MySolution.Application.Features.Application.Command.DeleteApplication;
using MySolution.Application.Features.Application.Command.UpdateApplication;
using MySolution.Application.Features.Application.Queries.GetApplicationById;
using MySolution.Application.Features.Application.Queries.GetApplications;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationController (IMediator mediator) : Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateApplication
    (
        [FromBody] CreateApplicationRequest request,
        CancellationToken cancellationToken 
    )
    {
       var response = await mediator.Send(new CreateApplicationCommand(request), cancellationToken);
       return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteApplication(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteApplicationCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateApplication
    (
        Guid id,
        [FromBody] UpdateApplicationRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdateApplicationCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetApplicationById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetApplications(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetApplicationsQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    //ApiKey:
    [HttpPost]
    [Route("applications/{applicationId}/api-keys")]
    public async Task<IActionResult> GenerateApiKey
    (
        GenerateApiKeyRequest request,
        Guid applicationId,
        CancellationToken cancellationToken
    )
    {
       var response = await mediator.Send(new GenerateApiKeyCommand(request,applicationId), cancellationToken);
       return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}