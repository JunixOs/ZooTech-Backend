using Microsoft.EntityFrameworkCore;
using ZooTech.Application;
using ZooTech.Application.Common.Gateway.Services;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;
using ZooTech.Infrastructure.Persistence.Context;
using ZooTech.InterfaceAdapters.Modules.Module_Tenancing.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(CeloController).Assembly)
    .AddApplicationPart(typeof(VacunoController).Assembly)
    .AddApplicationPart(typeof(ProduccionLecheController).Assembly);

// ======= Configuracion Swagger =======
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(TenancingController).Assembly);
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

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddInterfaceAdapters();

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

        // Si el puerto es el estandar (443 https, 80 http) no se incluye
        // en el origen, porque el navegador no lo envia en ese caso.
        var isStandardPort = frontendPort == "443" || frontendPort == "80";
        var origin = isStandardPort
            ? $"{frontendProtocol}://{frontendIP}"
            : $"{frontendProtocol}://{frontendIP}:{frontendPort}";

        policy.WithOrigins(origin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var app = builder.Build();

// ===== Configurar Middlewares =====
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TenantResolutionMiddleware>();
// ===== Configurar Middlewares =====

// ===== Configurar JWT =====
app.UseAuthentication();
app.UseAuthorization();
// ===== Configurar JWT =====


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
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
