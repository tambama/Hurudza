using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class LocalAuthorityViewModel : BaseViewModel
{
    public required string Name { get; set; }
    public string? DistrictId { get; set; }
    public string? District { get; set; }
}