using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class WardViewModel : BaseViewModel
{
    public required string Name { get; set; }
    public string? LocalAuthorityId { get; set; }
    public string? DistrictId { get; set; }
    public string? ProvinceId { get; set; }

    public string? LocalAuthority { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
}