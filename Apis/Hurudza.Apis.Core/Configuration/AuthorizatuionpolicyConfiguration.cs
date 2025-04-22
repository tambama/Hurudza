using Hurudza.Data.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Hurudza.Apis.Core.Configuration
{
    /// <summary>
    /// Configures permission-based authorization policies for the application
    /// </summary>
    public static class AuthorizationPolicyConfiguration
    {
        /// <summary>
        /// Configures all permission-based authorization policies
        /// </summary>
        public static void ConfigurePermissionPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Farm management policies
                options.AddPolicy("CanViewFarms", policy => 
                    policy.RequireClaim("Permission", Claims.FarmView));
                
                options.AddPolicy("CanCreateFarms", policy => 
                    policy.RequireClaim("Permission", Claims.FarmCreate));
                
                options.AddPolicy("CanManageFarms", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.FarmManage || c.Value == Claims.FarmCreate))));
                
                options.AddPolicy("CanDeleteFarms", policy => 
                    policy.RequireClaim("Permission", Claims.FarmDelete));
                
                // Field management policies
                options.AddPolicy("CanViewFields", policy => 
                    policy.RequireClaim("Permission", Claims.FieldView));
                
                options.AddPolicy("CanCreateFields", policy => 
                    policy.RequireClaim("Permission", Claims.FieldCreate));
                
                options.AddPolicy("CanManageFields", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.FieldManage || c.Value == Claims.FieldCreate))));
                
                options.AddPolicy("CanDeleteFields", policy => 
                    policy.RequireClaim("Permission", Claims.FieldDelete));
                
                // Crop management policies
                options.AddPolicy("CanViewCrops", policy => 
                    policy.RequireClaim("Permission", Claims.CropView));
                
                options.AddPolicy("CanCreateCrops", policy => 
                    policy.RequireClaim("Permission", Claims.CropCreate));
                
                options.AddPolicy("CanManageCrops", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.CropManage || c.Value == Claims.CropCreate))));
                
                options.AddPolicy("CanDeleteCrops", policy => 
                    policy.RequireClaim("Permission", Claims.CropDelete));
                
                // User management policies
                options.AddPolicy("CanViewUsers", policy => 
                    policy.RequireClaim("Permission", Claims.UserView));
                
                options.AddPolicy("CanCreateUsers", policy => 
                    policy.RequireClaim("Permission", Claims.UserCreate));
                
                options.AddPolicy("CanManageUsers", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.UserManage || c.Value == Claims.UserCreate))));
                
                options.AddPolicy("CanDeleteUsers", policy => 
                    policy.RequireClaim("Permission", Claims.UserDelete));
                
                // Role management policies
                options.AddPolicy("CanViewRoles", policy => 
                    policy.RequireClaim("Permission", Claims.RoleView));
                
                options.AddPolicy("CanCreateRoles", policy => 
                    policy.RequireClaim("Permission", Claims.RoleCreate));
                
                options.AddPolicy("CanManageRoles", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.RoleManage || c.Value == Claims.RoleCreate))));
                
                options.AddPolicy("CanDeleteRoles", policy => 
                    policy.RequireClaim("Permission", Claims.RoleDelete));
                
                // Tillage management policies
                options.AddPolicy("CanViewTillage", policy => 
                    policy.RequireClaim("Permission", Claims.TillageView));
                
                options.AddPolicy("CanCreateTillage", policy => 
                    policy.RequireClaim("Permission", Claims.TillageCreate));
                
                options.AddPolicy("CanManageTillage", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.TillageManage || c.Value == Claims.TillageCreate))));
                
                options.AddPolicy("CanDeleteTillage", policy => 
                    policy.RequireClaim("Permission", Claims.TillageDelete));
                
                // Combined policies for multiple features
                options.AddPolicy("CanManageSystem", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.UserManage || c.Value == Claims.RoleManage))));
                
                options.AddPolicy("CanViewOnly", policy => 
                    policy.RequireAssertion(context => 
                        context.User.HasClaim(c => c.Type == "Permission" && 
                            (c.Value == Claims.FarmView || 
                             c.Value == Claims.FieldView || 
                             c.Value == Claims.CropView ||
                             c.Value == Claims.TillageView))));
                
                // Role-based policies (as shortcuts for common scenarios)
                options.AddPolicy("IsSystemAdmin", policy => 
                    policy.RequireRole(ApiRoles.SystemAdministrator));
                
                options.AddPolicy("IsAdministrator", policy => 
                    policy.RequireAssertion(context => 
                        context.User.IsInRole(ApiRoles.SystemAdministrator) || 
                        context.User.IsInRole(ApiRoles.Administrator) ||
                        context.User.IsInRole(ApiRoles.FarmAdministrator)));
                
                options.AddPolicy("IsFarmManager", policy => 
                    policy.RequireAssertion(context => 
                        context.User.IsInRole(ApiRoles.SystemAdministrator) || 
                        context.User.IsInRole(ApiRoles.Administrator) ||
                        context.User.IsInRole(ApiRoles.FarmAdministrator) ||
                        context.User.IsInRole(ApiRoles.FarmManager)));
            });
        }
    }
}