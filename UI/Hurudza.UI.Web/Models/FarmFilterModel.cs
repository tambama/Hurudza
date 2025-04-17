// Filter Model class

using Hurudza.Data.Enums.Enums;

public class FarmFilterModel
{
    public string ProvinceId { get; set; }
    public string DistrictId { get; set; }
    public string LocalAuthorityId { get; set; }
    public Region? Region { get; set; }
    public Conference? Conference { get; set; }
    public float? MinSize { get; set; }
    public float? MaxSize { get; set; }
}