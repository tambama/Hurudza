using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement
{
    /// <summary>
    /// View model for assigning a user to a farm with a specific role
    /// </summary>
    public class FarmUserAssignmentViewModel
    {
        /// <summary>
        /// The ID of the user to assign to the farm
        /// </summary>
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        /// <summary>
        /// The ID of the farm to which the user will be assigned
        /// </summary>
        [Required(ErrorMessage = "Farm ID is required")]
        public string FarmId { get; set; }

        /// <summary>
        /// The role the user will have on this farm
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        /// <summary>
        /// Optional username for display purposes
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Optional full name for display purposes
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// Optional email for display purposes
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Optional farm name for display purposes
        /// </summary>
        public string FarmName { get; set; }
    }
}