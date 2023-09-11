using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class SendGridTemplate : BaseEntity
{
    public string TemplateId { get; set; }
    public SendGridTemplateName Name { get; set; }
}

public enum SendGridTemplateName
{
    DohweAccountConfirmation = 1,
    DohwePasswordResetCode,
    DohweGeneral,
}