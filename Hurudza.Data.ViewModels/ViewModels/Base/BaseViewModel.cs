using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.Base
{
    public class BaseViewModel
    {
        public string? Id { get; set; }

        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

        public bool Deleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatorId { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
