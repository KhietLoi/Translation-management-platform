using MassTransit;
using Microsoft.Extensions.Logging;
using MySolution.Application.Common.Interfaces;
using MySolution.Infrastructure.Options;
using Shared.MassTransit.IntegrationEvents;

namespace MySolution.Infrastructure.MassTransit.Consumers;

public class SendVerifyEmailConsumer : IConsumer<SendVerifyEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly IApplicationUrlProvider _applicationUrlProvider;

    public SendVerifyEmailConsumer(IEmailService emailService, IApplicationUrlProvider applicationUrlProvider)
    {
        _emailService = emailService;
        _applicationUrlProvider = applicationUrlProvider;
    }
    
    public async Task Consume(ConsumeContext<SendVerifyEmailEvent> context)
    {
        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] Consumer START");

        await Task.Delay(10000);

        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] Consumer END");
        var message = context.Message;
        var verifyUrl = _applicationUrlProvider.GetVerifyEmailUrl(message.Token);
        
        await _emailService.SendEmailAsync(
            message.Email,
            "Verify your email",
            $"<p>Please verify your email by clicking the link below:</p><p><a href='{verifyUrl}'>Verify Email</a></p>",
            cancellationToken: context.CancellationToken
        );
    }
}