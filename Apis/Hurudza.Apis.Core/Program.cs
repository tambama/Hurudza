using System.Reflection;
using System.Text;
using Hurudza.Apis.Base.Infrastructure.Filters;
using Hurudza.Apis.Base.Services;
using Hurudza.Common.Emails.Helpers;
using Hurudza.Common.Emails.Services;
using Hurudza.Common.Services.Interfaces;
using Hurudza.Common.Services.Services;
using Hurudza.Common.Sms.Services;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Seed;
using Hurudza.Data.Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.

    builder.Services.AddControllers();
    
    var assembly = Assembly.Load("Hurudza.Apis.Base");
    builder.Services.AddAutoMapper(assembly);
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddHttpClient();

    var connectionString = builder.Configuration.GetConnectionString("HurudzaConnection");

    // For Entity Framework
    builder.Services.AddDbContext<HurudzaDbContext>(options =>
        options.UseSqlServer(connectionString, x =>
        {
            x.MigrationsAssembly("Hurudza.Data.Context");
        }));

    // For Identity
    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<HurudzaDbContext>()
        .AddDefaultTokenProviders();
    
    // Adding Authentication  
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })

        // Adding Jwt Bearer  
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidAudience = builder.Configuration["JWT:ValidAudience"],
                ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? string.Empty))
            };
        });

    builder.Services.AddControllers(options =>
        {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });
    
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddServer(new OpenApiServer { Url = builder.Configuration["ApiSettings:CoreUrl"] });
        c.OperationFilter<SwaggerDefaultValues>();
    });
    builder.Services.AddApiVersioning();
    builder.Services.AddVersionedApiExplorer(options =>
    {
        options.DefaultApiVersion = ApiVersion.Parse("1.0");
        options.AssumeDefaultVersionWhenUnspecified = true;
    });
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddTransient<IDateTimeService, DateTimeService>();
    
    // Email Service
    builder.Services.Configure<EmailSettings>(options => builder.Configuration.GetSection(EmailSettings.Settings).Bind(options));
    builder.Services.AddTransient<ISendGridMessageHelper, SendGridMessageHelper>();
    builder.Services.AddTransient<ISendGridEmailService, SendGridEmailService>();

    // Sms Service
    builder.Services.Configure<SmsSettings>(options => builder.Configuration.GetSection(SmsSettings.Settings).Bind(options));
    builder.Services.AddTransient<ISmsService, ClickatellSmsService>();

    var app = builder.Build();

// Configure the HTTP request pipeline.
    using var scope = app.Services.CreateScope();
    var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
    var context = scope.ServiceProvider.GetRequiredService<HurudzaDbContext>();
    
    InitializeDatabase(app, context);
    
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DisplayOperationId();
            foreach (var description in provider.ApiVersionDescriptions.OrderByDescending(_ => _.ApiVersion))
            {
                c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Hurudza {description.GroupName}");
            }
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, ex.Message);
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}

static void InitializeDatabase(WebApplication app, HurudzaDbContext dbContext)
{
    using var serviceScope = app.Services.CreateScope();
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    dbContext.Database.Migrate();
    SeedRoleData.SeedRoles(dbContext, roleManager);
    SeedSendGridData.SeedSendGridTemplates(dbContext);
    SeedUserData.SeedUsers(dbContext, roleManager, userManager);
}