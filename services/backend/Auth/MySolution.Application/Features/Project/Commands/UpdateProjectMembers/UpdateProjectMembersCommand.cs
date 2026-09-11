using MediatR;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectMembers;

public class UpdateProjectMembersCommand : IRequest<UpdateProjectMembersResponse>
{
    public Guid ProjectId { get; set; }
    public UpdateProjectMembersRequest Payload { get; set; }

    public UpdateProjectMembersCommand(UpdateProjectMembersRequest payload, Guid projectId)
    {
        Payload = payload;
        ProjectId = projectId;
    }
}