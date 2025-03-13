using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

/// <summary>
/// Represents a body that owns a farm or property
/// </summary>
public class Entity : BaseEntity
{
    public EntityType EntityType { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    public string Description { get; set; }
    [Required(ErrorMessage = "Phone Number is required")]
    public string PhoneNumber { get; set; }
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    public virtual ICollection<FarmOwner> FarmOwners { get; set; }
}