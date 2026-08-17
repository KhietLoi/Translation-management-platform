namespace Shared.MassTransit.Contracts;

public class QueueNames
{
    public const string VerifyEmail = "send-verify-email-event";
    public const string SetupPasswordEmail = "send-set-up-password-email-event";
    public const string ForgotPasswordEmail = "send-forgot-password-email-event";
    public const string ExportTranslations = "export-translations-event";
    public const string ImportTranslations = "import-translations-event";
    public const string PublishTranslations = "publish-translations-event";
    public const string TranslationJobCompletedEmail = "translation-job-completed-email-event";
    public const string TranslationJobFailedEmail = "translation-job-failed-email-event";
    
}
