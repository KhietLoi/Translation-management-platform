namespace MySolution.Domain.Entities;

public class Application
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ApiKey> ApiKeys { get; set; }
        = new List<ApiKey>();
}