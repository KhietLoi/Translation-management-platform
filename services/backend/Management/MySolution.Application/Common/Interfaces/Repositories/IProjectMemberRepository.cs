using MySolution.Domain.Entities;

namespace MySolution.Application.Common.Interfaces.Repositories;

public interface IProjectMemberRepository : IRepository<ProjectMember>
{
    // Used for updating members
    Task<List<ProjectMember>> GetByProjectIdAsync(Guid projectId);

    // Used for querying members (includes User information)
    // Task<List<ProjectMember>> GetByProjectIdWithUserAsync(Guid projectId);

    // Used when tracking is required and includes User information
    Task<List<ProjectMember>> GetByProjectIdWithUserTrackingAsync(Guid projectId);
    
}