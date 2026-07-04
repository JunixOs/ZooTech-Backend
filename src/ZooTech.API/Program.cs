using Microsoft.EntityFrameworkCore;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(CeloController).Assembly)
    .AddApplicationPart(typeof(VacunoController).Assembly)
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
var frontendPort = builder.Configuration["Frontend:FrontendPort"];
var frontendIP = builder.Configuration["Frontend:FrontendIP"];
var frontendProtocol = builder.Configuration["Frontend:FrontendProtocol"];
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

var storagePath = Path.Combine(AppContext.BaseDirectory, "storage");
if (!Directory.Exists(storagePath))
{
    Directory.CreateDirectory(storagePath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(storagePath),
    RequestPath = "/api/v1/storage"
});

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");


// Middleware: Resolución de Tenant desde header X-Tenant-Id
// El TenantContext ya lee el header internamente vía IHttpContextAccessor,
// pero este log ayuda a diagnosticar qué tenant se está usando.
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("TenantMiddleware");

    if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
    {
        logger.LogInformation("Tenant recibido por header: {TenantId}", tenantHeader.ToString());
    }
    else
    {
        logger.LogInformation("Sin header X-Tenant-Id, usando DefaultTenantId del appsettings");
    }

    await next();
});
app.UseAuthorization();
app.MapControllers();

// ==========================================
// CÓDIGO DE CALENTAMIENTO (WARM-UP)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("WarmUp");
    
    try
    {
        logger.LogInformation("Iniciando calentamiento del modelo de Entity Framework Core...");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        var db = services.GetRequiredService<ZooTech.Infrastructure.Persistence.Context.GanaderiaDbContext>();
        
        if (await db.Database.CanConnectAsync())
        {
            await db.vacunos.AnyAsync();
            sw.Stop();
            logger.LogInformation("¡Calentamiento de Entity Framework completado con éxito en {ElapsedMs}ms!", sw.ElapsedMilliseconds);
        }
        else
        {
            sw.Stop();
            logger.LogWarning("No se pudo establecer conexión con la base de datos durante el calentamiento. Duración: {ElapsedMs}ms.", sw.ElapsedMilliseconds);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Ocurrió un error inesperado al calentar Entity Framework.");
    }
}

app.Run();
