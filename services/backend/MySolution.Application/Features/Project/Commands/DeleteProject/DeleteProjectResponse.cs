using System.Globalization;
using MySolution.Application.Common.Models;

namespace MySolution.Application.Features.Project.Commands.DeleteProject;

public class DeleteProjectResponse : BaseResponse <DeleteProjectData>
{
}

public class DeleteProjectData
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}