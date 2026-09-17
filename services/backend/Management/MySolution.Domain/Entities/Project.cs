namespace MySolution.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    //Insert OrganizationId property
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    
    public ICollection<ProjectLanguage> ProjectLanguages { get; set; } = new List<ProjectLanguage>();
    public ICollection<ProjectNamespace> ProjectNamespaces { get; set; } = new List<ProjectNamespace>();
    public ICollection<ProjectMember>  ProjectMembers { get; set; } = new List<ProjectMember>();
    public ICollection<TranslationKey> TranslationKeys { get; set; } = new List<TranslationKey>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<TranslationJob> TranslationJobs { get; set; } = new List<TranslationJob>();
    public ICollection<TranslationRelease> TranslationReleases { get; set; } = new List<TranslationRelease>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}