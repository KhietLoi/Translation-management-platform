using MySolution.Domain.Enums;

namespace MySolution.Domain.Entities;

public class TranslationValue
{
    public Guid Id { get; set; }
    public Guid TranslationKeyId { get; set; }
    public Guid LanguageId { get; set; }
    public string Value { get; set; } = string.Empty;
    public TranslationStatus Status { get; set; }
    public Guid? TranslatedBy { get; set; }
    public Guid? ReviewedBy { get; set; }
    public Guid? PublishedBy { get; set; }
    public DateTime? TranslatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public TranslationKey TranslationKey { get; set; } = null!;
    public Language Language { get; set; } = null!;
    public string? RejectionReason { get; set; }
    public User? Translator { get; set; }
    public User? Reviewer { get; set; }
    public User? Publisher { get; set; }
}