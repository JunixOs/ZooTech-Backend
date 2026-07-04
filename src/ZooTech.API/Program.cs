using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Controllers;
using ZooTech.InterfaceAdapters.DTOs.Responses;
using ZooTech.InterfaceAdapters.Middleware;
using ZooTech.InterfaceAdapters.Modules.Module_Celo.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_ProduccionLeche.Controllers;
using ZooTech.InterfaceAdapters.Modules.Module_Vacuno.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(HomeController).Assembly)
    .AddApplicationPart(typeof(CeloController).Assembly)
    .AddApplicationPart(typeof(VacunoController).Assembly)
    .AddApplicationPart(typeof(ProduccionLecheController).Assembly);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var details = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error => new ErrorDetail
            {
                Field = entry.Key,
                Message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                    ? "El valor enviado no es valido."
                    : error.ErrorMessage
            }));

        return new BadRequestObjectResult(ErrorResponse.Create(
            "VALIDATION_ERROR",
            "Los datos enviados no son validos.",
            details));
    };
});

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

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");

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

app.MapControllers();

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
            logger.LogInformation("Calentamiento de Entity Framework completado en {ElapsedMs}ms.", sw.ElapsedMilliseconds);
        }
        else
        {
            sw.Stop();
            logger.LogWarning("No se pudo establecer conexion con la base de datos durante el calentamiento. Duracion: {ElapsedMs}ms.", sw.ElapsedMilliseconds);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error inesperado al calentar Entity Framework.");
    }
}

app.Run();
