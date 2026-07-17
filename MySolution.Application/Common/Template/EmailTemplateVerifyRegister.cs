namespace MySolution.Application.Common.Templates;

public static class EmailTemplateVerifyRegister
{
    public static string VerifyEmail(
        string userName,
        string verifyUrl,
        int emailVerificationExpiryMinutes)
    {
        return $"""
                <h2>Welcome {userName}</h2>

                <p>Thank you for registering.</p>

                <p>Please verify your email by clicking the button below.</p>

                <a href="{verifyUrl}"
                   style="
                   background:#2563eb;
                   color:white;
                   padding:12px 20px;
                   text-decoration:none;
                   border-radius:6px;">
                   Verify Email
                </a>

                <p>This link will expire in {emailVerificationExpiryMinutes} minutes.</p>
                """;
    }
}