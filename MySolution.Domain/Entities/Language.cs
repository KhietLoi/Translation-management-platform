namespace MySolution.Domain.Entities;

public class Language
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<ProjectLanguage>  ProjectLanguages { get; set; } = new List<ProjectLanguage>();
}