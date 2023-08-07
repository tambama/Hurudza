using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Models.Enums;
using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Models.ViewModels.Core;

public class FarmViewModel
{
    public string? Id { get; set; }
    
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
    
    public int? WardId { get; set; }
    
    public int? DistrictId { get; set; }
    
    public int? LocalAuthorityId { get; set; }
    
    public int? ProvinceId { get; set; }
    
    public Region Region { get; set; }

    public string? Ward { get; set; }
    public string? LocalAuthority { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public List<FarmLocation> Locations { get; set; }
}