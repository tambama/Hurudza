using Hurudza.Data.Data;
using Hurudza.Data.Models.Models;

namespace Hurudza.Data.Context.Data;

/// <summary>
/// Permission categories for organization and management of claims
/// </summary>
public enum ApiClaimTypes
{
    Farm,
    Field,
    Crop,
    User,
    Role,
    Tillage,
    Equipment,
    Report,
    System
}

/// <summary>
/// Central repository of all application permission claims
/// </summary>
public static class ApiClaims
{
    /// <summary>
    /// Returns all application claims organized by category
    /// </summary>
    public static List<IdentityClaim> GetClaims()
    {
        return new List<IdentityClaim>()
        {
            // Farm permissions
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmView },
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmAdd },
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmCreate },
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmUpdate },
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmDelete },
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmManage },
            new() { ClaimType = ApiClaimTypes.Farm.ToString("G"), ClaimValue = Claims.FarmViewAll },
            
            // Field permissions
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldView },
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldCreate },
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldUpdate },
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldDelete },
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldManage },
            new() { ClaimType = ApiClaimTypes.Field.ToString("G"), ClaimValue = Claims.FieldViewAll },
            
            // Crop permissions
            new() { ClaimType = ApiClaimTypes.Crop.ToString("G"), ClaimValue = Claims.CropView },
            new() { ClaimType = ApiClaimTypes.Crop.ToString("G"), ClaimValue = Claims.CropCreate },
            new() { ClaimType = ApiClaimTypes.Crop.ToString("G"), ClaimValue = Claims.CropUpdate },
            new() { ClaimType = ApiClaimTypes.Crop.ToString("G"), ClaimValue = Claims.CropDelete },
            new() { ClaimType = ApiClaimTypes.Crop.ToString("G"), ClaimValue = Claims.CropManage },
            new() { ClaimType = ApiClaimTypes.Crop.ToString("G"), ClaimValue = Claims.CropViewAll },
            
            // User permissions
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserView },
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserCreate },
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserUpdate },
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserDelete },
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserManage },
            new() { ClaimType = ApiClaimTypes.User.ToString("G"), ClaimValue = Claims.UserViewAll },
            
            // Role permissions
            new() { ClaimType = ApiClaimTypes.Role.ToString("G"), ClaimValue = Claims.RoleView },
            new() { ClaimType = ApiClaimTypes.Role.ToString("G"), ClaimValue = Claims.RoleCreate },
            new() { ClaimType = ApiClaimTypes.Role.ToString("G"), ClaimValue = Claims.RoleUpdate },
            new() { ClaimType = ApiClaimTypes.Role.ToString("G"), ClaimValue = Claims.RoleDelete },
            new() { ClaimType = ApiClaimTypes.Role.ToString("G"), ClaimValue = Claims.RoleManage },
            
            // Tillage permissions
            new() { ClaimType = ApiClaimTypes.Tillage.ToString("G"), ClaimValue = Claims.TillageView },
            new() { ClaimType = ApiClaimTypes.Tillage.ToString("G"), ClaimValue = Claims.TillageCreate },
            new() { ClaimType = ApiClaimTypes.Tillage.ToString("G"), ClaimValue = Claims.TillageUpdate },
            new() { ClaimType = ApiClaimTypes.Tillage.ToString("G"), ClaimValue = Claims.TillageDelete },
            new() { ClaimType = ApiClaimTypes.Tillage.ToString("G"), ClaimValue = Claims.TillageManage },
            
            // Equipment permissions
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentView },
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentCreate },
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentUpdate },
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentEdit },
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentDelete },
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentManage },
            new() { ClaimType = ApiClaimTypes.Equipment.ToString("G"), ClaimValue = Claims.EquipmentViewAll },
            
            // Report permissions
            new() { ClaimType = ApiClaimTypes.Report.ToString("G"), ClaimValue = Claims.ReportView },
            new() { ClaimType = ApiClaimTypes.Report.ToString("G"), ClaimValue = Claims.ReportCreate },
            new() { ClaimType = ApiClaimTypes.Report.ToString("G"), ClaimValue = Claims.ReportExport },
            
            // System permissions
            new() { ClaimType = ApiClaimTypes.System.ToString("G"), ClaimValue = Claims.SystemSettings },
            new() { ClaimType = ApiClaimTypes.System.ToString("G"), ClaimValue = Claims.SystemAudit },
            new() { ClaimType = ApiClaimTypes.System.ToString("G"), ClaimValue = Claims.SystemBackup }
        };
    }
    
    /// <summary>
    /// Gets the display name for a permission claim
    /// </summary>
    public static string GetDisplayName(string claimValue)
    {
        // Format claim from "Entity.Action" to "Action Entity"
        if (claimValue.Contains('.'))
        {
            var parts = claimValue.Split('.');
            if (parts.Length == 2)
            {
                // Format the entity part (e.g., Farm -> Farms)
                string entity = parts[0];
                if (!entity.EndsWith("s") && entity != "System")
                {
                    entity += "s";
                }
                
                // Format the action part
                string action = parts[1];
                
                // Special case for ViewAll
                if (action == "ViewAll")
                {
                    return $"View All {entity}";
                }
                
                return $"{action} {entity}";
            }
        }
        
        return claimValue;
    }
    
    /// <summary>
    /// Gets the description for a permission claim
    /// </summary>
    public static string GetDescription(string claimValue)
    {
        return claimValue switch
        {
            // Farm permissions
            Claims.FarmView => "View farm details",
            Claims.FarmViewAll => "View all farms in the system",
            Claims.FarmCreate => "Create new farms",
            Claims.FarmAdd => "Add new farms under schools",
            Claims.FarmUpdate => "Update farm details",
            Claims.FarmDelete => "Delete farms",
            Claims.FarmManage => "Manage all farm details and settings",
            
            // Field permissions
            Claims.FieldView => "View field details",
            Claims.FieldViewAll => "View all fields across farms",
            Claims.FieldCreate => "Create new fields",
            Claims.FieldUpdate => "Update field details",
            Claims.FieldDelete => "Delete fields",
            Claims.FieldManage => "Manage all field details and settings",
            
            // Crop permissions
            Claims.CropView => "View crop details",
            Claims.CropViewAll => "View all crops across farms",
            Claims.CropCreate => "Create new crops",
            Claims.CropUpdate => "Update crop details",
            Claims.CropDelete => "Delete crops",
            Claims.CropManage => "Manage all crop details and settings",
            
            // User permissions
            Claims.UserView => "View user details",
            Claims.UserViewAll => "View all users in the system",
            Claims.UserCreate => "Create new users",
            Claims.UserUpdate => "Update user details",
            Claims.UserDelete => "Delete users",
            Claims.UserManage => "Manage all user accounts and permissions",
            
            // Role permissions
            Claims.RoleView => "View role details",
            Claims.RoleCreate => "Create new roles",
            Claims.RoleUpdate => "Update role details",
            Claims.RoleDelete => "Delete roles",
            Claims.RoleManage => "Manage all roles and permissions",
            
            // Tillage permissions
            Claims.TillageView => "View tillage details",
            Claims.TillageCreate => "Create new tillage programs",
            Claims.TillageUpdate => "Update tillage details",
            Claims.TillageDelete => "Delete tillage programs",
            Claims.TillageManage => "Manage all tillage programs and services",
            
            // Equipment permissions
            Claims.EquipmentView => "View equipment details",
            Claims.EquipmentViewAll => "View all equipment across farms",
            Claims.EquipmentCreate => "Create new equipment records",
            Claims.EquipmentUpdate => "Update equipment details",
            Claims.EquipmentEdit => "Edit equipment information",
            Claims.EquipmentDelete => "Delete equipment records",
            Claims.EquipmentManage => "Manage all equipment details and maintenance",
            
            // Report permissions
            Claims.ReportView => "View reports",
            Claims.ReportCreate => "Create new reports",
            Claims.ReportExport => "Export reports",
            
            // System permissions
            Claims.SystemSettings => "Manage system settings",
            Claims.SystemAudit => "View system audit logs",
            Claims.SystemBackup => "Manage system backups",
            
            _ => claimValue
        };
    }
}