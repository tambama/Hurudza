using Hurudza.Common.Emails.Models;
using SendGrid.Helpers.Mail;

namespace Hurudza.Common.Emails.Services;

public interface ISendGridEmailService
{
    public Task SendEmailAsync(string apiKey);
    public Task<EmailResult> SendSendGridEmailWithPersonalization(string apiKey, SendGridMessage emailObject);
}