using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class Crop : BaseEntity
{
    public required string Name { get; set; }
    
    public virtual ICollection<FieldCrop>? Fields { get; set; }
}