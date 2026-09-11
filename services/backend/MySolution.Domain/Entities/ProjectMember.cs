using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } 
    public Project Project { get; set; } = null!;
    public User User { get; set; } =  null!;
}