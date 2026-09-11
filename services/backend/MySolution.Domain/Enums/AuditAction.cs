namespace MySolution.Domain.Enums;

public enum AuditAction
{
    Create = 1,
    Update = 2,
    Delete = 3,
    SubmitTranslation = 4,
    ReviewTranslation = 5,
    RejectTranslation = 6,
    Import = 7,
    Export = 8,
    Publish = 9
}