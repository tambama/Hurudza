using System.Reflection;
using System.Text;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Hurudza.Apis.Base.Infrastructure.Filters;
using Hurudza.Apis.Base.Services;
using Hurudza.Apis.Core.Configuration;
using Hurudza.Common.Services.Interfaces;
using Hurudza.Common.Services.Services;
using Hurudza.Common.Sms.Services;
using Hurudza.Data.Context.Context;
using Hurudza.Data.Context.Seed;
using Hurudza.Data.Models.Models;
using Hurudza.Data.Services;
using Hurudza.Data.Services.Interfaces;
using Hurudza.Data.Services.Services;
using Hurudza.Data.UI.Models.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
        options.UseNpgsql(connectionString, x =>
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

    builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
        opt.TokenLifespan = TimeSpan.FromMinutes(5));

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

    // Configure authorization policies
    builder.Services.ConfigurePermissionPolicies();

    // Register application services
    builder.Services.AddScoped<IFarmUserManagerService, FarmUserManagerService>();
    builder.Services.AddScoped<IFarmUserAssignmentService, FarmUserAssignmentService>();
    builder.Services.AddSingleton<RoleInitializerService>();

    // Add hosted service for role initialization
    builder.Services.AddHostedService<RoleInitializerHostedService>();

    builder.Services.AddControllers(options =>
        {
            options.Filters.Add(typeof(HttpGlobalExceptionFilter));
        })
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("HurudzaCors",
            config =>
            {
                config
                    .WithOrigins("http://localhost:5113", "https://localhost:7148", "*")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(x => true)
                    .AllowCredentials();
            });
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddServer(new OpenApiServer { Url = builder.Configuration["ApiSettings:CoreUrl"] });
        c.OperationFilter<SwaggerDefaultValues>();
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddSwaggerGen(c =>
    {
        c.AddServer(new OpenApiServer { Url = builder.Configuration["ApiSettings:CoreUrl"] });
        c.OperationFilter<SwaggerDefaultValues>();
    });
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddTransient<IDateTimeService, DateTimeService>();

    // Sms Service
    builder.Services.Configure<SmsSettings>(options => builder.Configuration.GetSection(SmsSettings.Settings).Bind(options));
    builder.Services.AddTransient<ISmsService, ClickatellSmsService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<HurudzaDbContext>();
    var apiVersionDescriptionProvider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

    await InitializeDatabase(app, context);

    app.UseCors("HurudzaCors");

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DisplayOperationId();
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.OrderByDescending(_ => _.ApiVersion))
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

static async Task InitializeDatabase(WebApplication app, HurudzaDbContext dbContext)
{
    // Create a scope to resolve scoped services
    using var serviceScope = app.Services.CreateScope();
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleInitializer = serviceScope.ServiceProvider.GetRequiredService<RoleInitializerService>();

    dbContext.Database.Migrate();

    // Initialize roles and permissions
    await roleInitializer.InitializeRolesAndPermissions();

    // Seed initial data
    SeedClaimsData.SeedClaims(dbContext);
    SeedRoleData.SeedRoles(dbContext, roleManager);
    SeedSendGridData.SeedSendGridTemplates(dbContext);
    SeedUserData.SeedUsers(dbContext, roleManager, userManager);
    await SeedCropsData.SeedCrops(dbContext);
    if (Convert.ToBoolean(app.Configuration["Seed:AdminStructure"]))
    {
        await SeedAdministrativeStructureData.SeedAdministrativeStructure(dbContext);
    }
}