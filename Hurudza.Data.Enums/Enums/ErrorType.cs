using System.ComponentModel;

namespace Hurudza.Data.Enums.Enums
{
    public enum ErrorType
    {
        [Description("Invalid ID")]
        InvalidId,
        [Description("Invalid Address")]
        InvalidAddress,
        [Description("Invalid Date of Birth")]
        InvalidDateOfBirth,
        [Description("Full Name Required")]
        FullNameRequired,
        [Description("Incorrect Entity Name")]
        IncorrectName,
        [Description("Failed Save")]
        FailedSave,
        [Description("Worksheet Format Error")]
        WorksheetFormatError
    }
}
