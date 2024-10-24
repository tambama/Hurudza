using Hurudza.Data.Enums.Enums;
using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class FarmOwner : BaseEntity
{
    public string FarmId { get; set; }
    public string EntityId { get; set; }
    public OwnershipType OwnershipType { get; set; }
    public DateTime StartOfOwnership { get; set; }
    public DateTime? EndOfOwnership { get; set; }
    
    public virtual Farm Farm { get; set; }
    public virtual Entity Entity { get; set; }
}