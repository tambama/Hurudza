namespace Hurudza.Data.Models.Models;

public class Crop
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public virtual ICollection<FieldCrop>? Fields { get; set; }
}