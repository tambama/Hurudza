using Hurudza.Apis.Base.Infrastructure.Filters;
using Hurudza.Apis.Base.Services;
using Hurudza.Common.Services.Interfaces;
using Hurudza.Common.Services.Services;
using Hurudza.Data.Context.Context;
using Microsoft.AspNetCore.Datasync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HurudzaConnection");

if (connectionString == null)
{
    throw new ApplicationException("DefaultConnection is not set");
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();

builder.Services.AddDbContext<HurudzaDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatasyncControllers();

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
builder.Services.AddApiVersioning();
builder.Services.AddVersionedApiExplorer(options =>
{
    options.DefaultApiVersion = ApiVersion.Parse("1.0");
    options.AssumeDefaultVersionWhenUnspecified = true;
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();

var app = builder.Build();

// Configure and run the web service.
using var scope = app.Services.CreateScope();
var provider = scope.ServiceProvider.GetRequiredService<IApiVersionDescriptionProvider>();

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

app.MapControllers();
app.Run();

