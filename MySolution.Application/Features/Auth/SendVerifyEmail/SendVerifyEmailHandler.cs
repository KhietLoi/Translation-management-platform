using MediatR;
using MySolution.Application.Common.Interfaces;
using MySolution.Application.Common.Interfaces.Authentication;

namespace MySolution.Application.Features.Auth.SendVerifyEmail;

public class SendVerifyEmailHandler : IRequestHandler<SendVerifyEmailCommand>
{
    private readonly IApplicationUrlProvider _applicationUrlProvider;
    private readonly IEmailService _emailService;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ITokenSetting _tokenSettings;

    public SendVerifyEmailHandler
    (
        IEmailService emailService,
        IApplicationUrlProvider applicationUrlProvider,
        ITokenSetting tokenSetting,
        ITemplateRenderer templateRenderer
    )
    {
        _emailService = emailService;
        _applicationUrlProvider = applicationUrlProvider;
        _tokenSettings = tokenSetting;
        _templateRenderer = templateRenderer;
    }

    public async Task Handle(SendVerifyEmailCommand request, CancellationToken cancellationToken)
    {
        //await Task.Delay(10000, cancellationToken); 
        Console.WriteLine("Got to SendVerifyEmailHandler");
        /*Console.WriteLine(
            $"Handler executed: {DateTime.Now}");

        throw new Exception("TEST RETRY");*/

        var verifyUrl = _applicationUrlProvider.GetVerifyEmailUrl(request.Message.Token);
        /*var html = EmailTemplateVerifyRegister.VerifyEmail(
            request.Message.Username,
            verifyUrl,
            _tokenSettings.EmailVerificationExpiryMinutes
        );*/
        var html =
            await _templateRenderer.RenderAsync(
                "VerifyEmail.html",
                new
                {
                    user_name = request.Message.Username,
                    verify_url = verifyUrl,
                    expiry_minutes = _tokenSettings.EmailVerificationExpiryMinutes,
                    logo_url =
                        "https://wmtstorageaccdevsa.blob.core.windows.net/documents/019f82a5-9ee7-7e76-975b-fbdd3263c688.jpg",
                    year = DateTime.Now.Year
                },
                cancellationToken);

        await _emailService.SendEmailAsync(request.Message.Email, "Verify Your Email", html, cancellationToken);
    }
}