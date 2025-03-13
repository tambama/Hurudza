using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Context.Data;

public static class SendGridTemplates
{
    public static List<SendGridTemplate> GetTemplates()
    {
        return new List<SendGridTemplate>
        {
            new()
            {
                Name = SendGridTemplateName.DohweAccountConfirmation,
                TemplateId = "d-9ba3b1f3ff4047bd9f49bffb0e620a1c"
            },
            new()
            {
                Name = SendGridTemplateName.DohwePasswordResetCode,
                TemplateId = "d-96365266b3c246eaa2f60b458437cec9"
            },
            new()
            {
                Name = SendGridTemplateName.DohweGeneral,
                TemplateId = "d-224b5b6bbe304f28bf9728c5e6ddf244"
            },
        };
    }
}