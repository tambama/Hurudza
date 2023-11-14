using Hurudza.Apis.Base.Infrastructure.Filters;
using Hurudza.Apis.Base.Services;
using Hurudza.Common.Services.Interfaces;
using Hurudza.Common.Services.Services;
using Hurudza.Data.Context.Context;
using Microsoft.AspNetCore.Datasync;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
                .WithOrigins("http://localhost:5291", "https://localhost:7068", "https://fq57mn8s-7068.uks1.devtunnels.ms")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(x => true)
                .AllowCredentials();
        });
});

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IDateTimeService, DateTimeService>();

var app = builder.Build();

app.UseCors("HurudzaCors");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

