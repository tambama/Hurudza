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
using Hurudza.UI.Web.Services;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE2NTc3NUAzMjM1MmUzMDJlMzBuZVptV2VlNXVBdnB0di9RcVJ0NUZRT1EwWDdQZHZBcWJXWEdHWkZDTXZZPQ==");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CookieHandler>();
builder.Services.AddSingleton<JwtTokenService>();

builder.Services.AddHttpClient("Api.Core", options => {
        options.BaseAddress = new Uri("https://localhost:7148/api/");
    })
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore(options => {
    // Add custom policies based on permissions
    options.AddPolicy("CanManageFarms", policy => 
        policy.RequireClaim("Permission", "Farm.Manage"));
    
    options.AddPolicy("CanManageUsers", policy => 
        policy.RequireClaim("Permission", "User.Manage"));
    
    options.AddPolicy("CanManageFields", policy => 
        policy.RequireClaim("Permission", "Field.Manage"));
    
    options.AddPolicy("CanViewOnly", policy => 
        policy.RequireAssertion(context => 
            context.User.HasClaim(c => c.Type == "Permission" && 
                                     (c.Value == "Farm.View" || 
                                      c.Value == "User.View" || 
                                      c.Value == "Field.View"))));
    
    options.AddPolicy("IsSystemAdmin", policy => 
        policy.RequireClaim(ClaimTypes.Role, "SystemAdministrator"));
});

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IApiCall, ApiCall>();

builder.Services.AddSyncfusionBlazor();
await builder.Build().RunAsync();