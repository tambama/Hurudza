namespace Hurudza.Data.Models.Models;

public class Province
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public virtual ICollection<District>? Districts { get; set; }
    public virtual ICollection<Ward> Wards { get; set; }
    public virtual ICollection<Farm> Farms { get; set; }
}