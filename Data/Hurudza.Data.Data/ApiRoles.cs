namespace Hurudza.Data.Data
{
    /// <summary>
    /// Predefined application roles
    /// </summary>
    public static class ApiRoles
    {
        public const string SystemAdministrator = "SystemAdministrator";
        public const string Administrator = "Administrator";
        public const string FarmManager = "FarmManager";
        public const string FieldOfficer = "FieldOfficer";
        public const string Viewer = "Viewer";
        
        // Farm-specific roles (for users who only have access to specific farms)
        public const string FarmAdministrator = "FarmAdministrator";
        public const string FarmOperator = "FarmOperator";
        
        // Gets all roles organized by category
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
                        Administrator
                    }
                },
                {
                    "Farm", new List<string>
                    {
                        FarmManager,
                        FarmAdministrator,
                        FieldOfficer,
                        FarmOperator,
                        Viewer
                    }
                }
            };
        }
        
        // Gets the default permission claims for each role
        public static Dictionary<string, List<string>> GetDefaultRolePermissions()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    SystemAdministrator, new List<string>
                    {
                        // Full access to everything
                        Claims.FarmView, Claims.FarmCreate, Claims.FarmManage, Claims.FarmDelete,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldManage, Claims.FieldDelete,
                        Claims.CropView, Claims.CropCreate, Claims.CropManage, Claims.CropDelete,
                        Claims.UserView, Claims.UserCreate, Claims.UserManage, Claims.UserDelete,
                        Claims.RoleView, Claims.RoleCreate, Claims.RoleManage, Claims.RoleDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageManage, Claims.TillageDelete
                    }
                },
                {
                    Administrator, new List<string>
                    {
                        // Access to farms, fields and users, but not roles
                        Claims.FarmView, Claims.FarmCreate, Claims.FarmManage, Claims.FarmDelete,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldManage, Claims.FieldDelete,
                        Claims.CropView, Claims.CropCreate, Claims.CropManage, Claims.CropDelete,
                        Claims.UserView, Claims.UserCreate, Claims.UserManage, Claims.UserDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageManage, Claims.TillageDelete
                    }
                },
                {
                    FarmManager, new List<string>
                    {
                        // Farm-specific management
                        Claims.FarmView, Claims.FarmManage,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldManage, Claims.FieldDelete,
                        Claims.CropView, Claims.CropCreate, Claims.CropManage, Claims.CropDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageManage,
                        Claims.UserView
                    }
                },
                {
                    FarmAdministrator, new List<string>
                    {
                        // Farm-specific administration
                        Claims.FarmView, Claims.FarmManage,
                        Claims.FieldView, Claims.FieldCreate, Claims.FieldManage, Claims.FieldDelete,
                        Claims.CropView, Claims.CropCreate, Claims.CropManage, Claims.CropDelete,
                        Claims.TillageView, Claims.TillageCreate, Claims.TillageManage, Claims.TillageDelete,
                        Claims.UserView, Claims.UserManage
                    }
                },
                {
                    FieldOfficer, new List<string>
                    {
                        // Field operations only
                        Claims.FarmView,
                        Claims.FieldView, Claims.FieldManage,
                        Claims.CropView, Claims.CropCreate, Claims.CropManage,
                        Claims.TillageView, Claims.TillageCreate
                    }
                },
                {
                    FarmOperator, new List<string>
                    {
                        // Field operations only
                        Claims.FarmView,
                        Claims.FieldView,
                        Claims.CropView,
                        Claims.TillageView, Claims.TillageCreate
                    }
                },
                {
                    Viewer, new List<string>
                    {
                        // Read-only access
                        Claims.FarmView,
                        Claims.FieldView, 
                        Claims.CropView,
                        Claims.TillageView
                    }
                }
            };
        }
    }
}