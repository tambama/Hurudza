namespace Hurudza.Data.Data
{
    /// <summary>
    /// Predefined application roles
    /// </summary>
    public static class ApiRoles
    {
        // System Roles
        public const string SystemAdministrator = "SystemAdministrator";
        
        // General Roles
        public const string Administrator = "Administrator";
        
        // Farm-specific roles
        public const string FarmManager = "FarmManager";
        public const string FarmAdministrator = "FarmAdministrator";
        public const string FieldOfficer = "FieldOfficer";
        public const string FarmOperator = "FarmOperator";
        public const string Agronomist = "Agronomist";
        public const string Viewer = "Viewer";
        
        // New specialized roles
        public const string ReportingAnalyst = "ReportingAnalyst";
        public const string TillageManager = "TillageManager";
        
        /// <summary>
        /// Gets all roles organized by category
        /// </summary>
        public static Dictionary<string, List<string>> GetAllRoles()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    "System", new List<string>
                    {
                        SystemAdministrator
                    }
                },
                {
                    "General", new List<string>
                    {
                        Administrator,
                        ReportingAnalyst
                    }
                },
                {
                    "Farm", new List<string>
                    {
                        FarmManager,
                        FarmAdministrator,
                        FieldOfficer,
                        FarmOperator,
                        Agronomist,
                        TillageManager,
                        Viewer
                    }
                }
            };
        }
        
        /// <summary>
        /// Gets the default permission claims for each role
        /// </summary>
        public static Dictionary<string, List<string>> GetDefaultRolePermissions()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    SystemAdministrator, new List<string>
                    {
                        // Full access to everything
                        Claims.FarmView, Claims.FarmCreate, Claims.FarmUpdate, Claims.FarmManage, Claims.FarmDelete, Claims.FarmViewAll,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldUpdate, Claims.FieldManage, Claims.FieldDelete, Claims.FieldViewAll,
                        Claims.CropView, Claims.CropCreate, Claims.CropUpdate, Claims.CropManage, Claims.CropDelete, Claims.CropViewAll,
                        Claims.UserView, Claims.UserCreate, Claims.UserUpdate, Claims.UserManage, Claims.UserDelete, Claims.UserViewAll,
                        Claims.RoleView, Claims.RoleCreate, Claims.RoleUpdate, Claims.RoleManage, Claims.RoleDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate, Claims.TillageManage, Claims.TillageDelete,
                        Claims.ReportView, Claims.ReportCreate, Claims.ReportExport,
                        Claims.SystemSettings, Claims.SystemAudit, Claims.SystemBackup
                    }
                },
                {
                    Administrator, new List<string>
                    {
                        // Access to farms, fields, crops, users, and tillage, but not roles or system settings
                        Claims.FarmView, Claims.FarmCreate, Claims.FarmUpdate, Claims.FarmManage, Claims.FarmDelete, Claims.FarmViewAll,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldUpdate, Claims.FieldManage, Claims.FieldDelete, Claims.FieldViewAll,
                        Claims.CropView, Claims.CropCreate, Claims.CropUpdate, Claims.CropManage, Claims.CropDelete, Claims.CropViewAll,
                        Claims.UserView, Claims.UserCreate, Claims.UserUpdate, Claims.UserManage, Claims.UserDelete, Claims.UserViewAll,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate, Claims.TillageManage, Claims.TillageDelete,
                        Claims.ReportView, Claims.ReportCreate, Claims.ReportExport
                    }
                },
                {
                    FarmManager, new List<string>
                    {
                        // Farm-specific management
                        Claims.FarmView, Claims.FarmUpdate, Claims.FarmManage,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldUpdate, Claims.FieldManage, Claims.FieldDelete,
                        Claims.CropView, Claims.CropCreate, Claims.CropUpdate, Claims.CropManage, Claims.CropDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate, Claims.TillageManage,
                        Claims.UserView, Claims.UserViewAll,
                        Claims.ReportView, Claims.ReportCreate
                    }
                },
                {
                    FarmAdministrator, new List<string>
                    {
                        // Farm-specific administration
                        Claims.FarmView, Claims.FarmUpdate, Claims.FarmManage,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldUpdate, Claims.FieldManage, Claims.FieldDelete,
                        Claims.CropView, Claims.CropCreate, Claims.CropUpdate, Claims.CropManage, Claims.CropDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate, Claims.TillageManage, Claims.TillageDelete,
                        Claims.UserView, Claims.UserCreate, Claims.UserUpdate, Claims.UserManage,
                        Claims.ReportView, Claims.ReportCreate, Claims.ReportExport
                    }
                },
                {
                    FieldOfficer, new List<string>
                    {
                        // Field operations and crops
                        Claims.FarmView,
                        Claims.FieldView, Claims.FieldUpdate, Claims.FieldManage,
                        Claims.CropView, Claims.CropCreate, Claims.CropUpdate, Claims.CropManage,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate,
                        Claims.ReportView
                    }
                },
                {
                    FarmOperator, new List<string>
                    {
                        // Basic farm operations
                        Claims.FarmView,
                        Claims.FieldView,
                        Claims.CropView,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate,
                        Claims.ReportView
                    }
                },
                {
                    Agronomist, new List<string>
                    {
                        // Crop and field management
                        Claims.FarmView,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldUpdate, Claims.FieldManage,
                        Claims.CropView, Claims.CropCreate, Claims.CropUpdate, Claims.CropManage,
                        Claims.ReportView, Claims.ReportCreate
                    }
                },
                {
                    Viewer, new List<string>
                    {
                        // Read-only access
                        Claims.FarmView,
                        Claims.FieldView,
                        Claims.CropView,
                        Claims.TillageView,
                        Claims.ReportView
                    }
                },
                {
                    ReportingAnalyst, new List<string>
                    {
                        // Reporting functionality
                        Claims.FarmView, Claims.FarmViewAll,
                        Claims.FieldView, Claims.FieldViewAll,
                        Claims.CropView, Claims.CropViewAll,
                        Claims.TillageView,
                        Claims.ReportView, Claims.ReportCreate, Claims.ReportExport
                    }
                },
                {
                    TillageManager, new List<string>
                    {
                        // Tillage program specialist
                        Claims.FarmView,
                        Claims.FieldView,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageUpdate, Claims.TillageManage, Claims.TillageDelete,
                        Claims.ReportView, Claims.ReportCreate
                    }
                }
            };
        }
        
        /// <summary>
        /// Gets the display name for a role
        /// </summary>
        public static string GetDisplayName(string roleName)
        {
            return roleName switch
            {
                SystemAdministrator => "System Administrator",
                Administrator => "Administrator",
                FarmManager => "Farm Manager",
                FarmAdministrator => "Farm Administrator",
                FieldOfficer => "Field Officer",
                FarmOperator => "Farm Operator",
                Agronomist => "Agronomist",
                Viewer => "Viewer",
                ReportingAnalyst => "Reporting Analyst",
                TillageManager => "Tillage Manager",
                _ => roleName
            };
        }
        
        /// <summary>
        /// Gets the description for a role
        /// </summary>
        public static string GetDescription(string roleName)
        {
            return roleName switch
            {
                SystemAdministrator => "Full system access with ability to manage all aspects of the application",
                Administrator => "Administrative access to manage farms, users, and farm operations",
                FarmManager => "Manages all aspects of farm operations including fields, crops, and tillage",
                FarmAdministrator => "Manages farm operations and users assigned to the farm",
                FieldOfficer => "Manages field operations and crop data",
                FarmOperator => "Performs basic farm operations and data entry",
                Agronomist => "Specializes in crop and field management",
                Viewer => "Read-only access to view farm data",
                ReportingAnalyst => "Specializes in reporting and data analysis across all farms",
                TillageManager => "Specializes in tillage program management",
                _ => roleName
            };
        }
    }
}