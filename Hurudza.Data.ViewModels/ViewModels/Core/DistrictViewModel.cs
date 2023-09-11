using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core
{
    public class DistrictViewModel : BaseViewModel
    {
        public required string Name { get; set; }
        public required string ProvinceId { get; set; }
        public string? Province { get; set; }

    }
}