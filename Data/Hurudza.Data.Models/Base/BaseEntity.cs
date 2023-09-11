using Microsoft.AspNetCore.Datasync.EFCore;

namespace Hurudza.Data.Models.Base
{
    public class BaseEntity : EntityTableData
    {
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatorId { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
