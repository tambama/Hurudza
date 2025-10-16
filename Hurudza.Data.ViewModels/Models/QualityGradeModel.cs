using Hurudza.Data.Enums.Enums;

namespace Hurudza.Data.UI.Models.Models;

public class QualityGradeModel
{
    public QualityGrade Value { get; set; }
    public string Text => Value.ToString("G").Replace("_", " ");
    
    public QualityGradeModel(QualityGrade value)
    {
        Value = value;
    }
    
    public static List<QualityGradeModel> GetAll()
    {
        return Enum.GetValues<QualityGrade>()
            .Select(x => new QualityGradeModel(x))
            .ToList();
    }
}