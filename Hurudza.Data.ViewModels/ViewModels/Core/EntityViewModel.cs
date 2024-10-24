using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

/// <summary>
/// Represents a body that owns a farm or property
/// </summary>
public class EntityViewModel : BaseViewModel
{
    public EntityType EntityType { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required(ErrorMessage = "Phone Number is required")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}

public class EntityListViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
}