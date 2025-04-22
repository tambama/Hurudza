using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Hurudza.UI.Web;
using Hurudza.UI.Web.Api;
using Hurudza.UI.Web.Cookie.Handlers;
using Hurudza.UI.Web.Cookie.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor;
using Hurudza.UI.Shared.Api.Interfaces;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Services;
using IApiCall = Hurudza.UI.Web.Api.Interfaces.IApiCall;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE2NTc3NUAzMjM1MmUzMDJlMzBuZVptV2VlNXVBdnB0di9RcVJ0NUZRT1EwWDdQZHZBcWJXWEdHWkZDTXZZPQ==");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

// Authentication & Authorization
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddSingleton<JwtTokenService>();

// HTTP Clients
builder.Services.AddHttpClient("Api.Core", options => {
        options.BaseAddress = new Uri("https://localhost:7148/api/");
    })
    .AddHttpMessageHandler<CookieHandler>();

// Authorization Policies
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore(options => {
    // Farm management policies
    options.AddPolicy("CanViewFarms", policy => 
        policy.RequireClaim("Permission", "Farm.View"));
        
    options.AddPolicy("CanManageFarms", policy => 
        policy.RequireClaim("Permission", "Farm.Manage"));
    
    // Field management policies
    options.AddPolicy("CanViewFields", policy => 
        policy.RequireClaim("Permission", "Field.View"));
        
    options.AddPolicy("CanManageFields", policy => 
        policy.RequireClaim("Permission", "Field.Manage"));
    
    // Crop management policies
    options.AddPolicy("CanViewCrops", policy => 
        policy.RequireClaim("Permission", "Crop.View"));
        
    options.AddPolicy("CanManageCrops", policy => 
        policy.RequireClaim("Permission", "Crop.Manage"));
    
    // User management policies
    options.AddPolicy("CanViewUsers", policy => 
        policy.RequireClaim("Permission", "User.View"));
        
    options.AddPolicy("CanManageUsers", policy => 
        policy.RequireClaim("Permission", "User.Manage"));
    
    // Role-based policies
    options.AddPolicy("IsSystemAdmin", policy => 
        policy.RequireRole("SystemAdministrator"));
        
    options.AddPolicy("IsAdministrator", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("SystemAdministrator") || 
            context.User.IsInRole("Administrator") ||
            context.User.IsInRole("FarmAdministrator")));
            
    options.AddPolicy("IsFarmManager", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("SystemAdministrator") || 
            context.User.IsInRole("Administrator") ||
            context.User.IsInRole("FarmAdministrator") ||
            context.User.IsInRole("FarmManager")));
});

// Local Storage for JWT tokens
builder.Services.AddBlazoredLocalStorage();

// API Services
builder.Services.AddScoped<IApiCall, ApiCall>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// App Services
builder.Services.AddSingleton<SidebarToggleService>();
builder.Services.AddScoped<UserAssignmentService>();
builder.Services.AddScoped<FarmAccessService>();

// Syncfusion Blazor Components
builder.Services.AddSyncfusionBlazor();

await builder.Build().RunAsync();