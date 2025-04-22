using Hurudza.Data.Services;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hurudza.Apis.Core.Configuration
{
    /// <summary>
    /// Extension methods for registering application services
    /// </summary>
    public static class ServiceRegistrationExtensions
    {
        /// <summary>
        /// Registers all application services
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IFarmUserManagerService, FarmUserManagerService>();
            services.AddScoped<IFarmUserAssignmentService, FarmUserAssignmentService>();
            
            // Register singleton services
            services.AddSingleton<RoleInitializerService>();
            
            return services;
        }
    }
}