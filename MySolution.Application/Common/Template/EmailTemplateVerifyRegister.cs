namespace MySolution.Application.Common.Templates;

public static class EmailTemplateVerifyRegister
{
    public static string VerifyEmail(
        string userName,
        string verifyUrl,
        int emailVerificationExpiryMinutes,
        string logoUrl = "https://wmtstorageaccdevsa.blob.core.windows.net/documents/019f82a5-9ee7-7e76-975b-fbdd3263c688.jpg") 
    {
        return $"""
                <!DOCTYPE html>
                <html lang="en">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Email Verification</title>
                </head>
                <body style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; margin: 0; padding: 40px 20px;">
                    
                    <table width="100%" cellpadding="0" cellspacing="0" style="max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.05);">
                        
                        <!-- Header -->
                        <tr>
                            <td style="background-color: #2563eb; padding: 30px 20px; text-align: center;">
                                <h1 style="color: #ffffff; margin: 0; font-size: 24px; font-weight: 600;">Welcome to Our Platform!</h1>
                            </td>
                        </tr>
                        
                        <!-- Body Content -->
                        <tr>
                            <td style="padding: 40px 30px;">
                                <p style="font-size: 16px; color: #333333; margin-top: 0; margin-bottom: 20px;">Hello <strong>{userName}</strong>,</p>
                                
                                <p style="font-size: 15px; color: #555555; line-height: 1.6; margin-bottom: 30px;">
                                    Thank you for registering. To complete your setup and secure your account, please verify your email address by clicking the button below.
                                </p>
                                
                                <!-- Button -->
                                <div style="text-align: center; margin-bottom: 30px;">
                                    <a href="{verifyUrl}" 
                                       style="background-color: #2563eb; color: #ffffff; padding: 14px 30px; text-decoration: none; border-radius: 6px; font-weight: bold; display: inline-block; font-size: 16px;">
                                       Verify Email Address
                                    </a>
                                </div>
                                
                                <!-- Expiry Notice -->
                                <p style="font-size: 14px; color: #e11d48; text-align: center; margin-bottom: 30px; font-weight: 500;">
                                    ⏳ This link will expire in {emailVerificationExpiryMinutes} minutes.
                                </p>

                                <hr style="border: none; border-top: 1px solid #eaeaea; margin: 30px 0;">
                            </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                            <td style="background-color: #f9fafb; padding: 30px; text-align: center; border-top: 1px solid #eaeaea;">
                                
                                <!-- Thêm Logo ở đây -->
                                <div style="margin-bottom: 15px;">
                                    <!-- LOGO HEIGHT FIXED to 40px with width auto -->
                                    <img src="{logoUrl}" alt="MySolution Logo" style="max-height: 400px; width: 800px; display: block; margin: 0 auto; border: none;">
                                </div>

                                <p style="font-size: 12px; color: #888888; margin: 0; line-height: 1.5;">
                                    If you didn't request this email, you can safely ignore it.<br>
                                    &copy; {System.DateTime.Now.Year} MySolution. All rights reserved.
                                </p>
                            </td>
                        </tr>

                    </table>
                </body>
                </html>
                """;
    }
}