using Hurudza.Apis.Core.Configuration;
using Hurudza.Data.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hurudza.Apis.Core.Configuration
{
    /// <summary>
    /// Background service that runs during application startup to initialize roles and permissions
    /// </summary>
    public class RoleInitializerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public RoleInitializerHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a scope to resolve scoped services
            using var scope = _serviceProvider.CreateScope();
            var roleInitializer = scope.ServiceProvider.GetRequiredService<RoleInitializerService>();
            
            // Initialize roles and permissions
            await roleInitializer.InitializeRolesAndPermissions();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

namespace Hurudza.Apis.Core
{
    /// <summary>
    /// Startup configuration extensions
    /// </summary>
    public static class StartupConfiguration
    {
        /// <summary>
        /// Configures all services for the application
        /// </summary>
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            // Add application services
            services.AddApplicationServices();
            
            // Configure permission-based authorization policies
            services.ConfigurePermissionPolicies();
            
            // Add hosted services for initialization
            services.AddHostedService<RoleInitializerHostedService>();
            
            return services;
        }
    }
}