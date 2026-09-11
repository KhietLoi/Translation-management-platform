using System.Net;
using MediatR;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces.Repositories;
using MySolution.Domain.Entities;
namespace MySolution.Application.Features.Project.Commands.UpdateProjectMembers;

public class UpdateProjectMembersHandler : IRequestHandler<UpdateProjectMembersCommand, UpdateProjectMembersResponse>
{
    private readonly ILogger<UpdateProjectMembersHandler> _logger;
	private readonly IUnitOfWork _unitOfWork;

    public UpdateProjectMembersHandler
    (
        ILogger<UpdateProjectMembersHandler> logger,
		IUnitOfWork unitOfWork
    )
    {
        _logger = logger;
		_unitOfWork = unitOfWork;
    }

    #region Implementation of IRequestHandler<in UpdateProjectMembersCommand, UpdateProjectMembersResponse>

    public async Task<UpdateProjectMembersResponse> Handle(UpdateProjectMembersCommand request, CancellationToken cancellationToken)
    {
        var payload =  request.Payload;
        var functionName = $"{nameof(UpdateProjectMembersHandler)} =>";
        _logger.LogInformation(functionName);
        var response = new UpdateProjectMembersResponse();

        try
        {
            var project = await _unitOfWork.Project.GetByIdAsync(request.ProjectId);
            if (project == null) 
            {
                response.ErrorMessage = "Project not found";
                response.WithStatus(HttpStatusCode.NotFound);
                return response;
            }

            var currentMembers = await _unitOfWork.ProjectMember.GetByProjectIdAsync(request.ProjectId);
            var currentUserIds = currentMembers.Select(x => x.UserId).ToHashSet();
            var newMembers =
                payload.Members
                    .GroupBy(x => x.UserId)
                    .Select(x => x.First())
                    .ToList();
            var newUserIds = newMembers.Select(x => x.UserId).ToHashSet();
            // Add
            var userIdsToAdd = newUserIds.Except(currentUserIds).ToList();
            // Remove
            var membersToRemove =
                currentMembers
                    .Where(x => !newUserIds.Contains(x.UserId))
                    .ToList();
            // Validate users
            if (userIdsToAdd.Count > 0)
            {
                var users = await _unitOfWork.User.GetByIdsAsync(userIdsToAdd);
                var foundIds = users.Select(x => x.Id).ToHashSet();
                var invalidIds = userIdsToAdd.Except(foundIds).ToList();
                if (invalidIds.Count > 0)
                {
                    response.ErrorMessage = "One or more users do not exist";
                    response.WithStatus(HttpStatusCode.BadRequest);
                    return response;
                }

                var entities =
                    newMembers
                        .Where(x => userIdsToAdd.Contains(x.UserId))
                        .Select(x => new ProjectMember
                        {
                            ProjectId = request.ProjectId,
                            UserId = x.UserId,
                            CreatedAt = DateTime.UtcNow
                        })
                        .ToList();

                await _unitOfWork.ProjectMember.AddRange(entities);
            }
            
            // Remove
            if (membersToRemove.Count > 0)
            {
                _unitOfWork.ProjectMember.DeleteRange(membersToRemove);
            }
            
            await _unitOfWork.SaveAsync(cancellationToken);
            var updatedMembers = await _unitOfWork.ProjectMember.GetByProjectIdWithUserAsync(request.ProjectId);

            response.Data =
                new UpdateProjectMembersResult
                {
                    ProjectId = request.ProjectId,
                    ProjectName = project.Name,

                    Members =
                        updatedMembers
                            .Select(x => new UpdateProjectMembersData
                            {
                                UserId = x.UserId,
                                Username = x.User.Username,
                                Email = x.User.Email
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