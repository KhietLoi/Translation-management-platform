using Microsoft.Extensions.Options;
using MySolution.Email.Application.Common.Interfaces;
using MySolution.Email.Infrastructure.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MySolution.Email.Infrastructure.Services;

public class SendGridEmailService(IOptions<SendGridOptions> options) : IEmailService
{
    private readonly SendGridOptions _options = options.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent,
        CancellationToken cancellationToken = default)
    {
        //Client
        var client = new SendGridClient(_options.ApiKey);
        //From Email and name:
        var fromInfor = new EmailAddress(_options.FromEmail, _options.FromName);
        //Destination:
        var destination = new EmailAddress(toEmail);
        //Message:
        var message = MailHelper.CreateSingleEmail(fromInfor, destination, subject, "", htmlContent);
        //response:
        var response = await client.SendEmailAsync(message, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid Error: {body}");
        }
    }
}