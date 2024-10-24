using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class EntityUser : BaseEntity
{
    public string EntityId { get; set; }
    public string UserId { get; set; }
}