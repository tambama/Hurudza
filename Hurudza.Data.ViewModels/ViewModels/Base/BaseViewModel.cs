using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.Base
{
    public class BaseViewModel
    {
        public string? Id { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;


        [Timestamp]
        public byte[] Version { get; set; }

        public virtual bool Deleted { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatorId { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
