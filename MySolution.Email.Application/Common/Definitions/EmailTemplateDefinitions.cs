using MySolution.Email.Application.Common.Enums;

namespace MySolution.Email.Application.Common.Definitions;

public static class EmailTemplateDefinitions
{
    private static readonly Dictionary<EmailType, EmailTemplateDefinition> Definitions =
        new()
        {
            {
                EmailType.VerifyEmail,
                new(
                    "Verify Your Email",
                    "VerifyEmail.html")
            },

            {
                EmailType.ForgotPassword,
                new(
                    "Reset Password",
                    "ForgotPassword.html")
            },

            {
                EmailType.SetUpPassword,
                new(
                    "Set Up Your Password",
                    "SetUpPassword.html")
            },

            {
                EmailType.TranslationJobCompleted,
                new(
                    "Translation Job Completed",
                    "TranslationJobCompleted.html")
            }
            
        };

    public static EmailTemplateDefinition Get(EmailType emailType)
    {
        return Definitions[emailType];
    }
}