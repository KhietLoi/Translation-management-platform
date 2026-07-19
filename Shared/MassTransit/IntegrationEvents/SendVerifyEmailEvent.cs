namespace Shared.MassTransit.IntegrationEvents;

public class SendVerifyEmailEvent
{
    public Guid UserId { get; set; } = Guid .Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}