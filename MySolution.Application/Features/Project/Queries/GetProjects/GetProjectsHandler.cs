using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Project.Queries.GetProjects;

public class GetProjectsHandler : IRequestHandler<GetProjectsQuery, GetProjectsResponse>
{
    private readonly ILogger<GetProjectsHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetProjectsHandler
    (
        ILogger<GetProjectsHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetProjectsQuery, GetProjectsResponse>

    public async Task<GetProjectsResponse> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetProjectsHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProjectsResponse();

        try
        {
            var projects = await _unitOfWork.Project
                .GetAll()
                .AsNoTracking()
                .Select(x => new GetProjectData
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty,
                    IsActive = x.IsActive,
                    LanguageCount = x.ProjectLanguages.Count(),
                    MemberCount = x.ProjectMembers.Count(),
                    NamespaceCount = x.ProjectNamespaces.Count()
                })
                .ToListAsync(cancellationToken);

            response.Data = new GetProjectsResult
            {
                Projects = projects
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