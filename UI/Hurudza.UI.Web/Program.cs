using Blazored.LocalStorage;
using Hurudza.UI.Shared.Api;
using Hurudza.UI.Shared.Api.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Hurudza.UI.Web;
using Hurudza.UI.Web.Api;
using Hurudza.UI.Web.Api.Interfaces;
using Hurudza.UI.Web.Cookie.Handlers;
using Hurudza.UI.Web.Cookie.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Syncfusion.Blazor;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmVCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWXZccHRcRWZdVER3VkE=");

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<CookieHandler>();

builder.Services.AddHttpClient("Api.Core", options => {
        options.BaseAddress = new Uri("https://localhost:7148/api/");
    })
    .AddHttpMessageHandler<CookieHandler>();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IApiCall, ApiCall>();

builder.Services.AddSyncfusionBlazor();
await builder.Build().RunAsync();