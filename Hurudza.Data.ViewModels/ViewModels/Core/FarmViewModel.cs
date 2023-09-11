using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class FarmViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public required string Address { get; set; }

    [Required(ErrorMessage = "Phone Number is required")]
    [RegularExpression(@"^((00|\+)?(263))?0?7(1|3|7|8)[0-9]{7}$", ErrorMessage = "Enter a valid mobile number")]
    public required string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [DataType(DataType.EmailAddress)]
    public required string Email { get; set; }

    public float Size { get; set; }

    public string? WardId { get; set; }

    public string? DistrictId { get; set; }

    public string? LocalAuthorityId { get; set; }

    public string? ProvinceId { get; set; }

    public Region Region { get; set; }

    public string? Ward { get; set; }
    public string? LocalAuthority { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public List<FarmLocationViewModel> Locations { get; set; }
}