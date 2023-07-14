using SendGrid.Helpers.Mail;

namespace Hurudza.Common.Emails.Helpers;

public class SendGridMessageHelper : ISendGridMessageHelper
{
    private SendGridMessage _sendGridMessage;

    public SendGridMessageHelper()
    {
        _sendGridMessage = new SendGridMessage();
    }

    public SendGridMessage ConstructSendGridMessageObject(string toEmailAddress, string subject, string content, params string[] copies)
    {
        var message = new SendGridMessage();
        message.AddTo(toEmailAddress);
        message.Subject = subject;
        message.HtmlContent = content;
        message.TemplateId = "d-9ba3b1f3ff4047bd9f49bffb0e620a1c"; 
        foreach (var copy in copies)
        {
            _sendGridMessage.AddCc(copy);
        }

        return message;
    }

    public SendGridMessage ConstructNewSendGridMessageWithPersonalizations(string sendGridTemplateId, Dictionary<string, string> placeHolderDictionary, string toEmailAddress, string fromEmailAddress = "support@dohwe.com", params string[] copies)
    {
        _sendGridMessage = new SendGridMessage();
        _sendGridMessage.AddTo(toEmailAddress);
        _sendGridMessage.SetFrom(new EmailAddress(fromEmailAddress));
        _sendGridMessage.SetTemplateId(sendGridTemplateId);
        _sendGridMessage.SetTemplateData(placeHolderDictionary);
        foreach (var copy in copies)
        {
            _sendGridMessage.AddCc(copy);
        }

        return _sendGridMessage;
    }

    public SendGridMessage ConstructNewSendGridMessageWithAttachmentsAndPersonalizations(string sendGridTemplateId, Dictionary<string, string> placeHolderDictionary, string toEmailAddress, List<Tuple<string, MemoryStream>> attachments, string fromEmailAddress = "support@dohwe.com", params string[] copies)
    {
        _sendGridMessage = new SendGridMessage();
        _sendGridMessage.AddTo(toEmailAddress);
        _sendGridMessage.SetFrom(new EmailAddress(fromEmailAddress));
        _sendGridMessage.SetTemplateId(sendGridTemplateId);
        _sendGridMessage.SetTemplateData(placeHolderDictionary);

        foreach (var attachment in attachments)
        {
            var byteArray = attachment.Item2.ToArray();
            var file = Convert.ToBase64String(byteArray);
            _sendGridMessage.AddAttachment(attachment.Item1, file);
        }

        return _sendGridMessage;
    }

    public SendGridMessage ConstructNewSendGridMessageWithMemoryStreamAttachmentAndPersonalizations(string sendGridTemplateId, Dictionary<string, string> placeHolderDictionary, string toEmailAddress, string documentFriendlyName, MemoryStream attachment, string fromEmailAddress = "support@dohwe.com", params string[] copies)
    {
        _sendGridMessage = new SendGridMessage();
        var byteArray = attachment.ToArray();
        var file = Convert.ToBase64String(byteArray);

        _sendGridMessage.AddTo(toEmailAddress);
        _sendGridMessage.SetFrom(new EmailAddress(fromEmailAddress));
        _sendGridMessage.SetTemplateId(sendGridTemplateId);
        _sendGridMessage.SetTemplateData(placeHolderDictionary);
        _sendGridMessage.AddAttachment(documentFriendlyName, file);
        foreach (var copy in copies)
        {
            _sendGridMessage.AddCc(copy);
        }

        return _sendGridMessage;
    }
}