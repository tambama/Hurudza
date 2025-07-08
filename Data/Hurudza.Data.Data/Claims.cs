namespace Hurudza.Data.Data
{
    /// <summary>
    /// Predefined permission claims for the application
    /// </summary>
    public static class Claims
    {
        // Farm Permissions
        public const string FarmView = "Farm.View";
        public const string FarmAdd = "Farm.Add";
        public const string FarmCreate = "Farm.Create";
        public const string FarmUpdate = "Farm.Update";
        public const string FarmManage = "Farm.Manage";
        public const string FarmDelete = "Farm.Delete";
        public const string FarmViewAll = "Farm.ViewAll";
        
        // Field Permissions
        public const string FieldView = "Field.View";
        public const string FieldCreate = "Field.Create";
        public const string FieldUpdate = "Field.Update";
        public const string FieldManage = "Field.Manage";
        public const string FieldDelete = "Field.Delete";
        public const string FieldViewAll = "Field.ViewAll";
        
        // Crop Permissions
        public const string CropView = "Crop.View";
        public const string CropCreate = "Crop.Create";
        public const string CropUpdate = "Crop.Update";
        public const string CropManage = "Crop.Manage";
        public const string CropDelete = "Crop.Delete";
        public const string CropViewAll = "Crop.ViewAll";
        
        // User Permissions
        public const string UserView = "User.View";
        public const string UserCreate = "User.Create";
        public const string UserUpdate = "User.Update";
        public const string UserManage = "User.Manage";
        public const string UserDelete = "User.Delete";
        public const string UserViewAll = "User.ViewAll";
        
        // Role Permissions
        public const string RoleView = "Role.View";
        public const string RoleCreate = "Role.Create";
        public const string RoleUpdate = "Role.Update";
        public const string RoleManage = "Role.Manage";
        public const string RoleDelete = "Role.Delete";
        
        // Tillage Permissions
        public const string TillageView = "Tillage.View";
        public const string TillageCreate = "Tillage.Create";
        public const string TillageUpdate = "Tillage.Update";
        public const string TillageManage = "Tillage.Manage";
        public const string TillageDelete = "Tillage.Delete";
        
        // Report Permissions (new)
        public const string ReportView = "Report.View";
        public const string ReportCreate = "Report.Create";
        public const string ReportExport = "Report.Export";
        
        // System Permissions (new)
        public const string SystemSettings = "System.Settings";
        public const string SystemAudit = "System.Audit";
        public const string SystemBackup = "System.Backup";
        
        // Returns all available permissions grouped by feature
        public static Dictionary<string, List<string>> GetAllPermissions()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    "Farm", new List<string>
                    {
                        FarmView,
                        FarmCreate,
                        FarmUpdate,
                        FarmManage,
                        FarmDelete,
                        FarmViewAll
                    }
                },
                {
                    "Field", new List<string>
                    {
                        FieldView,
                        FieldCreate,
                        FieldUpdate,
                        FieldManage,
                        FieldDelete,
                        FieldViewAll
                    }
                },
                {
                    "Crop", new List<string>
                    {
                        CropView,
                        CropCreate,
                        CropUpdate,
                        CropManage,
                        CropDelete,
                        CropViewAll
                    }
                },
                {
                    "User", new List<string>
                    {
                        UserView,
                        UserCreate,
                        UserUpdate,
                        UserManage,
                        UserDelete,
                        UserViewAll
                    }
                },
                {
                    "Role", new List<string>
                    {
                        RoleView,
                        RoleCreate,
                        RoleUpdate,
                        RoleManage,
                        RoleDelete
                    }
                },
                {
                    "Tillage", new List<string>
                    {
                        TillageView,
                        TillageCreate,
                        TillageUpdate,
                        TillageManage,
                        TillageDelete
                    }
                },
                {
                    "Report", new List<string>
                    {
                        ReportView,
                        ReportCreate,
                        ReportExport
                    }
                },
                {
                    "System", new List<string>
                    {
                        SystemSettings,
                        SystemAudit,
                        SystemBackup
                    }
                }
            };
        }
        
        /// <summary>
        /// Gets all basic view permissions for read-only access
        /// </summary>
        public static List<string> GetBasicViewPermissions()
        {
            return new List<string>
            {
                FarmView,
                FieldView,
                CropView,
                TillageView,
                ReportView
            };
        }
        
        /// <summary>
        /// Gets all management permissions
        /// </summary>
        public static List<string> GetAllManagementPermissions()
        {
            return new List<string>
            {
                FarmManage,
                FieldManage,
                CropManage,
                UserManage,
                RoleManage,
                TillageManage,
                SystemSettings
            };
        }
    }
}