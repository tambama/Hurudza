using Hurudza.Common.Emails.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Hurudza.Common.Emails.Services;

public class SendGridEmailService : ISendGridEmailService
{
    public async Task SendEmailAsync(string apiKey)
    {
        try
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("support@dohwe.com", "Dohwe Support");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("tambama.peniel@outlook.com", "Peniel Tambama");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<EmailResult> SendSendGridEmailWithPersonalization(string apiKey, SendGridMessage emailObject)
    {
        var result = new EmailResult();
        if (!string.IsNullOrEmpty(apiKey))
        {
            try
            {
                var client = new SendGridClient(apiKey);
                var response = await client.SendEmailAsync(emailObject).ConfigureAwait(false);
                if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    result.Success = (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted);
                    result.Data = response;

                    if (!result.Success)
                    {
                        result.Message = "Message was not sent";
                    }
                }
            }
            catch (AggregateException err)
            {
                result.Message = "Unable to send email";
                foreach (var errInner in err.InnerExceptions)
                {
                    result.Data = err.InnerException;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
        }
        else
        {
            result.Message = "There was no API key";
        }

        return result;
    }
}