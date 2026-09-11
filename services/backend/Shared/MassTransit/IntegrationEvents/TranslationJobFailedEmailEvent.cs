namespace Shared.MassTransit.IntegrationEvents;

public class TranslationJobFailedEmailEvent
{
    public Guid JobId { get; set; }
    public Guid UserId { get; set; }            
    public string Email { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}