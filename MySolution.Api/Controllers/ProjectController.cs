using Microsoft.AspNetCore.Mvc;
using MySolution.Application.Features.Project.Commands.CreateProject;
using MediatR;
using MySolution.Api.Helpers;
using MySolution.Application.Features.Project.Commands.DeleteProject;
using MySolution.Application.Features.Project.Commands.UpdateProject;
using MySolution.Application.Features.Project.Queries.GetProjectById;
using MySolution.Application.Features.Project.Queries.GetProjects;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController (IMediator mediator): Controller
{
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new CreateProjectCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateProjectCommand(id,request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectsQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}