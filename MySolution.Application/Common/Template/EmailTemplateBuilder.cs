namespace MySolution.Application.Common.Templates;

public static class EmailTemplateBuilder
{
    public static string WelcomeEmail(
        string userName)
    {
        return $"""
                <html>
                <body>

                    <h2>Xin chào {userName}</h2>

                    <p>
                        Tài khoản của bạn đã được tạo thành công.
                    </p>

                    <p>
                        Chúc bạn sử dụng hệ thống vui vẻ.
                    </p>

                </body>
                </html>
                """;
    }
}