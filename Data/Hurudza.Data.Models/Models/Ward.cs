namespace Hurudza.Data.Models.Models;

public class Ward
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required District District { get; set; }
    public int DistrictId { get; set; }
}