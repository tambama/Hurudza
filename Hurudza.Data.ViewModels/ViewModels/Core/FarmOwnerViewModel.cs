using Hurudza.Data.Enums.Enums;
using Hurudza.Data.UI.Models.ViewModels.Base;

namespace Hurudza.Data.UI.Models.ViewModels.Core;

public class FarmOwnerViewModel : BaseViewModel
{
    public string FarmId { get; set; }
    public string EntityId { get; set; }
    public OwnershipType OwnershipType { get; set; }
    public DateTime StartOfOwnership { get; set; }
    public DateTime? EndOfOwnership { get; set; }
    public string? Farm { get; set; }
    public string? Entity { get; set; }
}

public class CreateFarmOwnerViewModel
{
    public string Id { get; set; }
    public string FarmId { get; set; }
    public string EntityId { get; set; }
    public OwnershipType OwnershipType { get; set; }
    public DateTime StartOfOwnership { get; set; }
    public DateTime? EndOfOwnership { get; set; }
}