using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Context.Data;

public static class ApplicationRoles
{
    public static List<ApplicationRole> GetApplicationRoles()
    {
        return new List<ApplicationRole>
        {
            new()
            {
                Name = ApiRoles.SystemAdministrator,
                Description = "System Administrator",
                RoleClass = RoleClass.System,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FarmView},
                    
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserDelete},
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserManage},
                    
                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = ApiClaims.FieldView},
                }
            },
            new()
            {
                Name = ApiRoles.Administrator,
                Description = "Administrator",
                RoleClass = RoleClass.System,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FarmDelete},
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FarmManage},

                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserDelete},
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserManage},

                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = ApiClaims.FieldDelete},
                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = ApiClaims.FieldManage},
                }
            },
            new()
            {
                Name = ApiRoles.Agronomist,
                Description = "Agronomist",
                RoleClass = RoleClass.General,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FarmManage },
                    
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserView},

                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FieldDelete}, 
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FieldManage}, 
                }
            },
            new()
            {
                Name = ApiRoles.FarmManager,
                Description = "Farm Manager",
                RoleClass = RoleClass.Farm,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = ApiClaims.FarmManage },
                    
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserDelete},
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = ApiClaims.UserManage},

                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = ApiClaims.FieldDelete}, 
                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = ApiClaims.FieldManage}, 
                }
            },
        };
    }
}
