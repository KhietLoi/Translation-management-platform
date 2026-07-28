using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
namespace MySolution.Application.Features.Project.Queries.GetProjectMembers;

public class GetProjectMembersHandler : IRequestHandler<GetProjectMembersQuery, GetProjectMembersResponse>
{
    private readonly ILogger<GetProjectMembersHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public GetProjectMembersHandler
    (
        ILogger<GetProjectMembersHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in GetProjectMembersQuery, GetProjectMembersResponse>

    public async Task<GetProjectMembersResponse> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
    {
        var functionName = $"{nameof(GetProjectMembersHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new GetProjectMembersResponse();

        try
        {   
            //Check project is already exists
            var isProject = await _unitOfWork.Project.ExistsAsync(request.ProjectId);
            if (!isProject)
            {
                response.ErrorMessage = "Project not found.";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }
            
            var members = await _unitOfWork.ProjectMember.GetByProjectIdWithUserAsync(request.ProjectId);

            response.Data = new GetProjectMembersResult
            {
                Members = members.Select(x => new GetProjectMemberData
                {
                    UserId = x.UserId,
                    Username = x.User.Username,
                    Email = x.User.Email,
                    Role = x.Role
                }).ToList()
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