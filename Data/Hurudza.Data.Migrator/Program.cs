// See https://aka.ms/new-console-template for more information
using Common.Services.Services;
using Hurudza.Common.Services.Interfaces;
using Hurudza.Common.Services.Services;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Migrator.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

try
{
    using IHost builder = Host.CreateDefaultBuilder(args).Build();

    // Ask the service provider for the configuration abstraction.
    IConfiguration config = builder.Services.GetRequiredService<IConfiguration>();

    var services = new ServiceCollection();
    string connectionString = "Server=localhost;Port=5432;Database=Hurudza;Username=postgres;Password=Password+1;";
    // For Entity Framework
    services.AddDbContext<HurudzaDbContext>(options =>
        options.UseNpgsql(connectionString, x =>
        {
            x.MigrationsAssembly("Hurudza.Data.Context");
        }));

    services.AddHttpContextAccessor();

    services.AddTransient<IConfiguration>(sp =>
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");
        return configurationBuilder.Build();
    });

    services.AddTransient<ICurrentUserService, CurrentUserService>();
    services.AddTransient<IDateTimeService, DateTimeService>();
    services.AddScoped<AdminStructureSetupService>();
    services.AddLogging();

    var serviceProvider = services.BuildServiceProvider();

    var _context = serviceProvider.GetService<HurudzaDbContext>();
    var _adminStructureSetupService = serviceProvider.GetService<AdminStructureSetupService>();

    Stopwatch stopwatch = Stopwatch.StartNew();
    stopwatch.Start();

    Console.WriteLine("Loading voters...");

    await _adminStructureSetupService.ImportBulkVoters().ConfigureAwait(false);

    stopwatch.Stop();
    Console.WriteLine("Time taken: {0}", stopwatch.ElapsedMilliseconds / 60000);

    Console.ReadLine();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Shut down complete");
}

