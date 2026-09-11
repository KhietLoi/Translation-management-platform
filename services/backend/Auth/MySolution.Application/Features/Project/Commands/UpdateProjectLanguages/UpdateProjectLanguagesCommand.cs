using MediatR;

namespace MySolution.Application.Features.Project.Commands.UpdateProjectLanguages;

public class UpdateProjectLanguagesCommand : IRequest<UpdateProjectLanguagesResponse>
{
    public UpdateProjectLanguagesRequest Payload { get; set; }
    public Guid ProjectId { get; set; }

    public UpdateProjectLanguagesCommand(UpdateProjectLanguagesRequest payload, Guid projectId)
    {
        Payload = payload;
        ProjectId = projectId;
    }
}