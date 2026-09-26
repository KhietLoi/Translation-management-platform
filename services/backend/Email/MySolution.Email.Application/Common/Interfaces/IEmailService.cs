namespace MySolution.Email.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken cancellationToken = default);
}