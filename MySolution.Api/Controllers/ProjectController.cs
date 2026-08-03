using Microsoft.AspNetCore.Mvc;
using MySolution.Application.Features.Project.Commands.CreateProject;
using MediatR;
using MySolution.Api.Authorization;
using MySolution.Api.Helpers;
using MySolution.Application.Constants;
using MySolution.Application.Features.Project.Commands.CreateProjectNamespace;
using MySolution.Application.Features.Project.Commands.DeleteProject;
using MySolution.Application.Features.Project.Commands.DeleteProjectNamespace;
using MySolution.Application.Features.Project.Commands.UpdateProject;
using MySolution.Application.Features.Project.Commands.UpdateProjectLanguages;
using MySolution.Application.Features.Project.Commands.UpdateProjectMembers;
using MySolution.Application.Features.Project.Commands.UpdateProjectNamespace;
using MySolution.Application.Features.Project.Queries.GetProjectById;
using MySolution.Application.Features.Project.Queries.GetProjectLanguages;
using MySolution.Application.Features.Project.Queries.GetProjectMembers;
using MySolution.Application.Features.Project.Queries.GetProjectNamspaces;
using MySolution.Application.Features.Project.Queries.GetProjects;

namespace MySolution.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectController (IMediator mediator): Controller
{
    [HttpPost]
    [Permission(PermissionConstants.Project.Create)]
    public async Task<IActionResult> CreateProject
    (
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new CreateProjectCommand(request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [Permission(PermissionConstants.Project.View)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectById(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpPut("{id:guid}")]
    [Permission(PermissionConstants.Project.Update)]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateProjectCommand(id,request), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpDelete("{id:guid}")]
    [Permission(PermissionConstants.Project.Delete)]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }

    [HttpGet]
    [Permission(PermissionConstants.Project.View)]
    public async Task<IActionResult> GetProjects(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectsQuery(), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("{projectId:guid}/namespaces")]
    [Permission(PermissionConstants.Project.View)]
    public async Task<IActionResult> GetProjectNamespaces(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectNamspacesQuery(projectId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPost("{projectId:guid}/namespaces")]
    [Permission(PermissionConstants.Project.Create)]
    public async Task<IActionResult> CreateProjectNamespace
    (
        [FromBody] CreateProjectNamespaceRequest request,
        Guid projectId,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new CreateProjectNamespaceCommand(request, projectId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("{projectId:guid}/namespaces/{id:guid}")]
    [Permission(PermissionConstants.Project.Update)]
    public async Task<IActionResult> UpdateProjectNamespace
    (
        Guid id, 
        [FromBody] UpdateProjectNamespaceRequest request, 
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new UpdateProjectNamespaceCommand(request, id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpDelete("{projectId:guid}/namespaces/{id:guid}")]
    [Permission(PermissionConstants.Project.Delete)]
    public async Task<IActionResult> DeleteProjectNamespace(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeleteProjectNamespaceCommand(id), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("{projectId:guid}/languages")]
    [Permission(PermissionConstants.Project.Update)]
    public async Task<IActionResult> UpdateProjectLanguages
    (
        Guid projectId,
        [FromBody] UpdateProjectLanguagesRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateProjectLanguagesCommand(request, projectId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpPut("{projectId:guid}/members")]
    [Permission(PermissionConstants.Project.Update)]
    public async Task<IActionResult> UpdateProjectMembers
    (
        Guid projectId,
        [FromBody] UpdateProjectMembersRequest request,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new UpdateProjectMembersCommand(request, projectId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
    
    [HttpGet("{projectId:guid}/languages")]
    [Permission(PermissionConstants.Project.View)]
    public async Task<IActionResult> GetProjectLanguages(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectLanguagesQuery(projectId), cancellationToken);
        return ResponseHelper.ToResponse( response.StatusCode, response, response.Data);
    }

    [HttpGet("{projectId:guid}/members")]
    [Permission(PermissionConstants.Project.View)]
    public async Task<IActionResult> GetProjectMembers(Guid projectId, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetProjectMembersQuery(projectId), cancellationToken);
        return ResponseHelper.ToResponse(response.StatusCode, response, response.Data);
    }
}