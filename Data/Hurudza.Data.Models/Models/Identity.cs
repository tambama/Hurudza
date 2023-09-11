using Hurudza.Data.Enums.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hurudza.Data.Models.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser: IdentityUser
    {
        public required string Firstname { get; set; }
        public required string Surname { get; set; }
        public string Fullname => $"{Firstname} {Surname}";
        
        public string? RegistrationToken { get; set; }
        
        public DateTime? TokenValidity { get; set; }
        
        // Audit
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatorId { get; set; }
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        public string? ModifiedBy { get; set; }
        public bool IsArchived { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<ApplicationUserClaim>? Claims { get; set; }
        public virtual ICollection<ApplicationUserLogin>? Logins { get; set; }
        public virtual ICollection<ApplicationUserToken>? Tokens { get; set; }
        public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public required string Description { get; set; }
        
        public RoleClass RoleClass { get; set; }

        public virtual ICollection<ApplicationUserRole>? UserRoles { get; set; }
        public virtual ICollection<ApplicationRoleClaim>? RoleClaims { get; set; }
    }

    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser? User { get; set; }
        public virtual ApplicationRole? Role { get; set; }
    }

    public class ApplicationUserClaim : IdentityUserClaim<string>
    {
        public virtual ApplicationUser? User { get; set; }
    }

    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser? User { get; set; }
    }

    public class ApplicationRoleClaim : IdentityRoleClaim<string>
    {
        public ApplicationRole? Role { get; set; }
    }

    public class ApplicationUserToken : IdentityUserToken<string>
    {
        public virtual ApplicationUser? User { get; set; }
    }

    public class IdentityClaim
    {
        public int Id { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
