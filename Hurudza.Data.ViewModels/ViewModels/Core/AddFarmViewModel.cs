// Hurudza.Data.ViewModels/ViewModels/Core/AddFarmViewModel.cs

using System.ComponentModel.DataAnnotations;
using Hurudza.Data.UI.Models.ViewModels.Base;

public class AddFarmViewModel : BaseViewModel
{
    [Required(ErrorMessage = "Farm name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }
    
    public string? ContactPerson { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    public string PhoneNumber { get; set; }
    
    public float Size { get; set; }
    public float Arable { get; set; }
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public bool RequiresTillageService { get; set; } = true;
    public string? TillageRequirements { get; set; }
    public string? CropRotationPlan { get; set; }
    
    // Hidden field to store the parent school ID
    public string ParentSchoolId { get; set; }
}