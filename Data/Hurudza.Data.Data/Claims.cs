namespace Hurudza.Data.Data
{
    /// <summary>
    /// Predefined permission claims for the application
    /// </summary>
    public static class Claims
    {
        // Farm Permissions
        public const string FarmView = "Farm.View";
        public const string FarmCreate = "Farm.Create";
        public const string FarmManage = "Farm.Manage";
        public const string FarmDelete = "Farm.Delete";
        
        // Field Permissions
        public const string FieldView = "Field.View";
        public const string FieldCreate = "Field.Create";
        public const string FieldManage = "Field.Manage";
        public const string FieldDelete = "Field.Delete";
        
        // Crop Permissions
        public const string CropView = "Crop.View";
        public const string CropCreate = "Crop.Create";
        public const string CropManage = "Crop.Manage";
        public const string CropDelete = "Crop.Delete";
        
        // User Permissions
        public const string UserView = "User.View";
        public const string UserCreate = "User.Create";
        public const string UserManage = "User.Manage";
        public const string UserDelete = "User.Delete";
        
        // Role Permissions
        public const string RoleView = "Role.View";
        public const string RoleCreate = "Role.Create";
        public const string RoleManage = "Role.Manage";
        public const string RoleDelete = "Role.Delete";
        
        // Tillage Permissions
        public const string TillageView = "Tillage.View";
        public const string TillageCreate = "Tillage.Create";
        public const string TillageManage = "Tillage.Manage";
        public const string TillageDelete = "Tillage.Delete";
        
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
                        FarmManage,
                        FarmDelete
                    }
                },
                {
                    "Field", new List<string>
                    {
                        FieldView,
                        FieldCreate,
                        FieldManage,
                        FieldDelete
                    }
                },
                {
                    "Crop", new List<string>
                    {
                        CropView,
                        CropCreate,
                        CropManage,
                        CropDelete
                    }
                },
                {
                    "User", new List<string>
                    {
                        UserView,
                        UserCreate,
                        UserManage,
                        UserDelete
                    }
                },
                {
                    "Role", new List<string>
                    {
                        RoleView,
                        RoleCreate,
                        RoleManage,
                        RoleDelete
                    }
                },
                {
                    "Tillage", new List<string>
                    {
                        TillageView,
                        TillageCreate,
                        TillageManage,
                        TillageDelete
                    }
                }
            };
        }
    }
}