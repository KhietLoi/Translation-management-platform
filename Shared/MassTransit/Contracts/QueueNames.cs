namespace Shared.MassTransit.Contracts;

public class QueueNames
{
    public const string VerifyEmail = "send-verify-email-event";
    public const string SetupPasswordEmail = "send-set-up-password-email-event";
    public const string ForgotPasswordEmail = "send-forgot-password-email-event";
}
