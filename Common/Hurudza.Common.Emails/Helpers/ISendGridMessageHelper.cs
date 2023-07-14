using SendGrid.Helpers.Mail;

namespace Hurudza.Common.Emails.Helpers;

public interface ISendGridMessageHelper
{
    SendGridMessage ConstructSendGridMessageObject(string toEmailAddress, string subject, string content, params string[] copies);
    SendGridMessage ConstructNewSendGridMessageWithPersonalizations(string sendGridTemplateId, Dictionary<string, string> placeHolderDictionary, string toEmailAddress, string fromEmailAddress = "noreply@dohwe.com", params string[] copies);
    SendGridMessage ConstructNewSendGridMessageWithAttachmentsAndPersonalizations(string sendGridTemplateId, Dictionary<string, string> placeHolderDictionary, string toEmailAddress, List<Tuple<string, MemoryStream>> attachments, string fromEmailAddress = "noreply@dohwe.com", params string[] copies);
    SendGridMessage ConstructNewSendGridMessageWithMemoryStreamAttachmentAndPersonalizations(string sendGridTemplateId, Dictionary<string, string> placeHolderDictionary, string toEmailAddress, string documentFriendlyName, MemoryStream attachment, string fromEmailAddress = "noreply@dohwe.com", params string[] copies);
}