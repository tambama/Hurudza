namespace Hurudza.Data.Models.Models;

public class SendGridTemplate
{
    public int Id { get; set; }
    public string TemplateId { get; set; }
    public SendGridTemplateName Name { get; set; }
}

public enum SendGridTemplateName
{
    DohweAccountConfirmation = 1,
    DohwePasswordResetCode,
    DohweGeneral,
}