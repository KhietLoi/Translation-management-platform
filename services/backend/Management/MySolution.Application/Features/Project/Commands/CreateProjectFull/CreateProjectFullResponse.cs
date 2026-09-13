using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.CreateProjectFull;

public class CreateProjectFullResponse : BaseResponse <CreateProjectFullData>
{
}

public class CreateProjectFullData
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
}
