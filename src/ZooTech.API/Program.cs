using System.Reflection;
using Microsoft.Extensions.FileProviders;
using ZooTech.Application;
using ZooTech.Infrastructure;
using ZooTech.InterfaceAdapters;
using ZooTech.InterfaceAdapters.Middleware;

// ─────────────────────────────────────────────────────────────────────────────
// ZooTech API – TK02: GET /vacunos/reportes/listado
// Proyecto aislado y funcional para la tarea TK02.
// Diseñado con Clean Architecture para futura integración al sistema principal.
// ─────────────────────────────────────────────────────────────────────────────

var builder = WebApplication.CreateBuilder(args);

// ── Capas de la arquitectura ─────────────────────────────────────────────────
builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddInterfaceAdapters();

// ── Controladores ─────────────────────────────────────────────────────────────
// Se descubren los controladores de ZooTech.InterfaceAdapters
builder.Services.AddControllers()
    .AddApplicationPart(
        typeof(ZooTech.InterfaceAdapters.Modules.Module_ReporteVacuno
               .Controllers.ReportesVacunosController).Assembly);

// ── Swagger / OpenAPI ─────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "ZOO | Módulo Vacuno – TK02",
        Version = "v1",
        Description = "Endpoint TK02: GET /vacunos/reportes/listado – " +
                      "Reporte listado de vacunos con filtros, búsqueda y paginación."
    });

    // Incluir comentarios XML del proyecto API
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    // Incluir comentarios XML de InterfaceAdapters
    var adaptersXml = Path.Combine(
        AppContext.BaseDirectory,
        "ZooTech.InterfaceAdapters.xml");
    if (File.Exists(adaptersXml))
        options.IncludeXmlComments(adaptersXml);
});

// ── Logging ───────────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// ── CORS ──────────────────────────────────────────────────────────────────────
// En desarrollo y producción se controla desde configuración o variables de entorno:
// Cors__AllowedOrigins__0=https://tu-frontend.com
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCors", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>()
            ?? Array.Empty<string>();

        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // En producción, no abrir CORS si no se configuró explícitamente.
            policy.WithOrigins("https://frontend-no-configurado.local")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// ─────────────────────────────────────────────────────────────────────────────
// Pipeline HTTP
// ─────────────────────────────────────────────────────────────────────────────

// 1. Middleware global de manejo de excepciones (debe ir primero)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Swagger UI: desarrollo por defecto, o por variable Swagger__Enabled=true
var swaggerEnabled = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooTech TK02 v1");
        c.RoutePrefix = string.Empty; // Swagger en raíz: http://localhost:5085
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Permite exponer los archivos generados en wwwroot/reportes/vacunos
var generatedFilesRoot = Path.Combine(AppContext.BaseDirectory, "wwwroot");
Directory.CreateDirectory(generatedFilesRoot);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(generatedFilesRoot),
    RequestPath = string.Empty
});

app.UseCors("FrontendCors");
app.MapControllers();

app.Run();
