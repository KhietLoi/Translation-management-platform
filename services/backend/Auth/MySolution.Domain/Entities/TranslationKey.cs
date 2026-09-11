namespace MySolution.Domain.Entities;

public class TranslationKey
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid NamespaceId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public  Project Project { get; set; } = null!;
    public  ProjectNamespace Namespace { get; set; } = null!;
    public  ICollection<TranslationValue> TranslationValues { get; set; } = new List<TranslationValue>();
}