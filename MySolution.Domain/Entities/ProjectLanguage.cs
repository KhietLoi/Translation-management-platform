namespace MySolution.Domain.Entities;

public class ProjectLanguage
{
    public Guid LanguageId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public Language Language { get; set; } = null!;
    public Project Project { get; set; } = null!;
}