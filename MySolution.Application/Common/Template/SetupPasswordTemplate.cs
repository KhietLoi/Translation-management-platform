namespace MySolution.Application.Common.Templates;

public class SetupPasswordTemplate
{
    public static string SetupPassword(
        string userName,
        string setupUrl,
        int passwordResetExpiryMinutes)
    {
        return $@"
            <h2>Welcome {userName}</h2>

            <p>
                An administrator has created an account for you.
            </p>

            <p>
                Please click the button below to set your password.
            </p>

            <a href='{setupUrl}'>
                Set Password
            </a>

            <p>
                This link will expire in {passwordResetExpiryMinutes} minutes.
            </p>";
    }
}