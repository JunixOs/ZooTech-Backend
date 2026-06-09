using Microsoft.EntityFrameworkCore;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.Infrastructure.Configuration;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;

var builder = WebApplication.CreateBuilder(args);

// ======= Configuracion Swagger =======
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(ProduccionLecheController).Assembly);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("auth", new()
    {
        Title = "Authentication API",
        Version = "v1"
    });

    options.SwaggerDoc("users", new()
    {
        Title = "Users API",
        Version = "v1"
    });

    options.SwaggerDoc("public", new()
    {
        Title = "Public API",
        Version = "v1"
    });
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddInterfaceAdapters();

builder.Services.AddSanidadServices(builder.Configuration);

// ======= Configuracion Context BD Tenant Principal =======
builder.Services.AddDbContext<TenantCatalogDb>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("TenantCatalogConnection"));
});
// ======= Configuracion Context BD Tenant Principal =======

// ======= Configuracion DI =======
builder.Services.AddMemoryCache();
// ======= Configuracion DI =======

var frontendPort = builder.Configuration["Frontend:FrontendPort"] ?? "5000";
var frontendIP = builder.Configuration["Frontend:FrontendIP"] ?? "localhost";
var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"] ?? "http";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var frontendPort = builder.Configuration["Frontend:FrontendPort"];
        var frontendIP = builder.Configuration["Frontend:FrontendIP"];
        var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"];

        policy.WithOrigins($"{frontendProtocol}://{frontendIP}:{frontendPort}")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===== Configurar Middlewares =====
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
// ===== Configurar Middlewares =====

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/public/swagger.json",
            "Public API");

        options.SwaggerEndpoint(
            "/swagger/auth/swagger.json",
            "Authentication API");

        options.SwaggerEndpoint(
            "/swagger/users/swagger.json",
            "Users API");
    });
}

//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
