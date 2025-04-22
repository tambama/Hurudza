using System.ComponentModel.DataAnnotations;

namespace Hurudza.Data.UI.Models.ViewModels.UserManagement
{
    /// <summary>
    /// View model for resetting a user's password
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// The username of the user whose password is being reset
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        
        /// <summary>
        /// The new password
        /// </summary>
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        /// <summary>
        /// Optional user ID if resetting password by ID instead of username
        /// </summary>
        public string UserId { get; set; }
        
        /// <summary>
        /// Optional reset token when resetting through email link
        /// </summary>
        public string ResetToken { get; set; }
    }
}