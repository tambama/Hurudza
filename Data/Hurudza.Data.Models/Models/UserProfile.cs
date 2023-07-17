using Hurudza.Data.Models.Base;

namespace Hurudza.Data.Models.Models;

public class UserProfile: BaseEntity
{
    public string Id { get; set; }
    public required string UserId { get; set; }
    public required string FarmId { get; set; }
    public required string Role { get; set; }

    public virtual Farm? Farm { get; set; }
    public virtual ApplicationUser? User { get; set; }
}
