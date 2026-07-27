using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Project.Queries.GetProjectById;

public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, GetProjectByIdResponse>
{
    private readonly ILogger<GetProjectByIdHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetProjectByIdHandler
    (
        ILogger<GetProjectByIdHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetProjectByIdQuery, GetProjectByIdResponse>

    public async Task<GetProjectByIdResponse> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetProjectByIdHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProjectByIdResponse();

        try
        {
            var project = await _unitOfWork.Project.GetDetailAsync(request.ProjectId);
            if (project == null)
            {
                response.ErrorMessage = "Project not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            response.Data = new GetProjectIdData
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                UpdatedAt = project.UpdatedAt,
                ProjectLanguages = project.ProjectLanguages
                    .Select(x => new GetProjectLanguageData
                    {
                        LanguageId = x.LanguageId,
                        Code = x.Language.Code,
                        Name = x.Language.Name
                    }).ToList(),
                ProjectMembers = project.ProjectMembers
                    .Select(x => new GetProjectMemberData
                    {
                        UserId = x.UserId,
                        UserName = x.User.Username,
                        Email = x.User.Email,
                        Role = x.Role
                    }).ToList(),
                ProjectNamespaces = project.ProjectNamespaces
                    .Select(x => new GetProjectNamespaceData
                    {
                        NamespaceId = x.Id,
                        Name = x.Name
                    })
                    .ToList()
            };
            response
                .WithSuccess(true)
                .WithStatus(HttpStatusCode.OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} Unexpected error.", functionName);
            response.ErrorMessage = "An unexpected error occurred.";
            response.WithStatus(HttpStatusCode.InternalServerError);
        }

        return response;
    }

    #endregion
}