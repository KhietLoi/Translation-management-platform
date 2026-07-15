namespace MySolution.Application.Common.Templates;

public class ResetPasswordTemplate
{
    public static string ResetPassword(
        string userName,
        string resetUrl,
        int passwordResetExpiryHours)
    {
        return $"""
                <h2>Hello {userName}</h2>

                <p>Click below to reset your password.</p>

                <a href="{resetUrl}">
                    Reset Password
                </a>

                <p>This link expires in {passwordResetExpiryHours} hour.</p>
                """;
    }
}