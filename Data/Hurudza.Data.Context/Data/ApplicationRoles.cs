using Hurudza.Data.Data;
using Hurudza.Data.Enums.Enums;
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
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmView},
                    
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserDelete},
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserManage},
                    
                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldView},
                }
            },
            new()
            {
                Name = ApiRoles.Administrator,
                Description = "Administrator",
                RoleClass = RoleClass.System,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmCreate},
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmDelete},
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmManage},

                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserDelete},
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserManage},

                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldDelete},
                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldManage},
                    
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentCreate},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentUpdate},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentEdit},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentDelete},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentManage},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentViewAll},
                }
            },
            new()
            {
                Name = ApiRoles.Agronomist,
                Description = "Agronomist",
                RoleClass = RoleClass.General,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmManage },
                    
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserView},

                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FieldDelete}, 
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FieldManage}, 
                }
            },
            new()
            {
                Name = ApiRoles.FarmManager,
                Description = "Farm Manager",
                RoleClass = RoleClass.Farm,
                RoleClaims = new List<ApplicationRoleClaim>
                {
                    new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmManage },
                    
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserDelete},
                    new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserManage},

                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldDelete}, 
                    new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldManage},
                    
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentCreate},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentUpdate},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentEdit},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentDelete},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentManage},
                    new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentView},
                }
            },
        };
    }
}
