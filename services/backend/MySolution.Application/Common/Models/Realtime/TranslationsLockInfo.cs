namespace MySolution.Application.Common.Models.Realtime;

public class TranslationsLockInfo
{
    public Guid TranslationValueId { get; set;}
    public Guid UserId { get; set;}
    public string Username { get; set;} = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public DateTime LockedAt { get; set;}
}