using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class TransactionTypeModel
{
    public TransactionType Value { get; set; }
    public string Text { get; set; }

    public TransactionTypeModel()
    {
    }

    public TransactionTypeModel(TransactionType value)
    {
        Value = value;
        Text = value.ToString("G").Replace("_", " ");
    }
}