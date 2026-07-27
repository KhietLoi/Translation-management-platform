namespace MySolution.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<ProjectLanguage> ProjectLanguages { get; set; } = new List<ProjectLanguage>();
    public ICollection<ProjectNamespace> ProjectNamespaces { get; set; } = new List<ProjectNamespace>();
    public ICollection<ProjectMember>  ProjectMembers { get; set; } = new List<ProjectMember>();
}